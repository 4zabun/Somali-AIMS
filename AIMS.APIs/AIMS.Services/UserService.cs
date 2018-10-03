﻿using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns></returns>
        IEnumerable<UserView> GetAll();

        /// <summary>
        /// Gets all users async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserView>> GetAllAsync();

        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAuthenticationView AuthenticateUser(string email, string password);

        /// <summary>
        /// Checks availability of email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        ActionResponse CheckEmailAvailability(string email);

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(UserModel user, SmtpClient smtp, string adminEmail);

        /// <summary>
        /// Updates a user's organization
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ActionResponse UpdateOrganization(int userId, int organizationId);

        /// <summary>
        /// Updates a user's organization
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        ActionResponse UpdatePassword(int userId, string newPassword);

        /// <summary>
        /// Deletes the account for the provided user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        ActionResponse Delete(string email, string password);
    }

    public class UserService : IUserService
    {
        AIMSDbContext context;
        IMapper mapper;

        public UserService(AIMSDbContext cntxt, IMapper mappr)
        {
            this.context = cntxt;
            this.mapper = mappr;
        }

        public IEnumerable<UserView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<UserView> usersList = new List<UserView>();
                var users = unitWork.UserRepository.GetWithInclude(u => u.Id != 0, new string[] { "Organization" });
                usersList = mapper.Map<List<UserView>>(users);
                return usersList;
            }
        }

        public async Task<IEnumerable<UserView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<UserView> usersList = new List<UserView>();
                var users = await unitWork.UserRepository.GetAllAsync();
                usersList = mapper.Map<List<UserView>>(users);
                return await Task<IEnumerable<UserView>>.Run(() => usersList).ConfigureAwait(false);
            }
        }

        public UserAuthenticationView AuthenticateUser(string email, string password)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                UserAuthenticationView foundUser = new UserAuthenticationView();
                var findUser = unitWork.UserRepository.GetWithInclude(u => u.Email.Equals(email) && u.Password.Equals(password),
                    new string[] { "Organization" });

                if (findUser != null)
                {
                    foreach (var user in findUser)
                    {
                        foundUser.Id = user.Id;
                        foundUser.Email = user.Email;
                        foundUser.DisplayName = user.DisplayName;
                        foundUser.UserType = user.UserType;
                        foundUser.OrganizationId = user.Organization.Id;
                        break;
                    }
                }
                return foundUser;
            }
        }

        public ActionResponse CheckEmailAvailability(string email)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var user = unitWork.UserRepository.Get(u => u.Email.Equals(email));
                if (user != null)
                {
                    response.Success = false;
                    return response;
                }
                return response;
            }
        }

        public ActionResponse Add(UserModel model, SmtpClient smtp, string adminEmail)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    EFOrganization organization = null;
                    ISecurityHelper sHelper = new SecurityHelper();
                    IMessageHelper mHelper;

                    if (!model.IsNewOrganization)
                    {
                        organization = unitWork.OrganizationRepository.GetByID(model.OrganizationId);
                        if (organization == null)
                        {
                            mHelper = new MessageHelper();
                            response.Success = false;
                            response.Message = mHelper.GetNotFound("Organization");
                            return response;
                        }
                    }
                    else
                    {
                        EFOrganizationTypes organizationType = null;
                        if (model.IsNewOrganization)
                        {
                            organizationType = unitWork.OrganizationTypesRepository.Get(o => o.Id.Equals(model.OrganizationTypeId));
                            if (organizationType == null)
                            {
                                mHelper = new MessageHelper();
                                response.Success = false;
                                response.Message = mHelper.GetNotFound("Organization Type");
                                return response;
                            }

                            organization = new EFOrganization()
                            {
                                OrganizationName = model.OrganizationName,
                                OrganizationType = organizationType
                            };

                            unitWork.Save();
                            model.OrganizationId = organization.Id;
                        }
                    }

                    string passwordHash = sHelper.GetPasswordHash(model.Password);
                    var newUser = unitWork.UserRepository.Insert(new EFUser()
                    {
                        DisplayName = model.DisplayName,
                        Email = model.Email,
                        UserType = model.UserType,
                        Organization = organization,
                        Password = passwordHash,
                        IsApproved = false,
                        RegistrationDate = DateTime.Now
                    });
                    unitWork.Save();
                    //Get emails for all the users
                   /* var users = unitWork.UserRepository.GetMany(u => u.OrganizationId.Equals(organization.Id) || u.UserType.Equals(UserTypes.Manager));
                    List<EmailsModel> usersEmailList = new List<EmailsModel>();
                    foreach (var user in users)
                    {
                        usersEmailList.Add(new EmailsModel()
                        {
                            Email = user.Email,
                            UserName = user.DisplayName,
                            UserType = user.UserType
                        });
                    }
                    //Send emails
                    IEmailHelper emailHelper = new EmailHelper(smtp, adminEmail);
                    emailHelper.SendNewRegistrationEmail(usersEmailList, organization.OrganizationName);*/
                    response.ReturnedId = newUser.Id;
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse UpdateOrganization(int userId, int organizationId)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var organization = unitWork.OrganizationRepository.GetByID(organizationId);
                IMessageHelper msgHelper;
                if (organization == null)
                {
                    msgHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = msgHelper.GetNotFound("Organization");
                    return response;
                }

                var user = unitWork.UserRepository.GetByID(userId);
                if (user == null)
                {
                    msgHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = msgHelper.GetNotFound("User");
                    return response;
                }

                user.Organization = organization;
                unitWork.UserRepository.Update(user);
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }

        public ActionResponse UpdatePassword(int userId, string password)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper msgHelper;
                var user = unitWork.UserRepository.GetByID(userId);
                if (user == null)
                {
                    msgHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = msgHelper.GetNotFound("User");
                    return response;
                }

                ISecurityHelper sHelper = new SecurityHelper();
                user.Password = sHelper.GetPasswordHash(password);
                unitWork.UserRepository.Update(user);
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }

        public ActionResponse Delete(string email, string password)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                ISecurityHelper sHelper = new SecurityHelper();
                string passwordHash = sHelper.GetPasswordHash(password);
                var user = unitWork.UserRepository.Get(u => u.Email.Equals(email) && u.Password.Equals(password));

                IMessageHelper mHelper;
                if (user == null)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("User");
                    return response;
                }

                if (user.UserType == UserTypes.SuperAdmin)
                {
                    mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.InvalidAccountDeletionAttempt();
                    return response;
                }

                unitWork.UserRepository.Delete(user);
                unitWork.Save();
                return response;
            }
        }
    }
}
