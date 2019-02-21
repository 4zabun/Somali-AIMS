﻿using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AIMS.Services
{
    public interface IOrganizationService
    {
        /// <summary>
        /// Gets all organizations
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrganizationView> GetAll();

        /// <summary>
        /// Gets organization by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OrganizationViewModel Get(int id);

        /// <summary>
        /// Gets all organizations matching the name with criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IEnumerable<OrganizationView> GetMatching(string criteria);

        /// <summary>
        /// Gets all organizations async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OrganizationView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(OrganizationModel organization);

        /// <summary>
        /// Merges two organizations
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ActionResponse> MergeOrganizations(MergeOrganizationModel model);

        /// <summary>
        /// Updates a organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        ActionResponse Update(int id, OrganizationModel organization);

        /// <summary>
        /// Approves an organization with the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ActionResponse Approve(int id);
    }

    public class OrganizationService : IOrganizationService
    {
        AIMSDbContext context;
        IMapper mapper;

        public OrganizationService(AIMSDbContext cntxt, IMapper mappr)
        {
            context = cntxt;
            this.mapper = mappr;
        }

        public IEnumerable<OrganizationView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationView> organizationsList = new List<OrganizationView>();
                var organizations = unitWork.OrganizationRepository.GetMany(o => o.Id != 0);
                return mapper.Map<List<OrganizationView>>(organizations);
            }
        }

        public OrganizationViewModel Get(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var organizationList = unitWork.OrganizationRepository.GetMany(o => o.Id.Equals(id));
                EFOrganization organization = null;
                foreach(var org in organizationList)
                {
                    organization = org;
                }
                return mapper.Map<OrganizationViewModel>(organization);
            }
        }

        public IEnumerable<OrganizationView> GetMatching(string criteria)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                List<OrganizationView> organizationsList = new List<OrganizationView>();
                var organizations = unitWork.OrganizationRepository.GetMany(o => o.OrganizationName.Contains(criteria));
                return mapper.Map<List<OrganizationView>>(organizations);
            }
        }

        public async Task<IEnumerable<OrganizationView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var organizations = await unitWork.OrganizationRepository.GetAsync(o => o.Id != 0);
                return await Task<IEnumerable<OrganizationView>>.Run(() => mapper.Map<List<OrganizationView>>(organizations)).ConfigureAwait(false);
            }
        }

        public ActionResponse Add(OrganizationModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var isOrganizationCreated = unitWork.OrganizationRepository.GetOne(o => o.OrganizationName.ToLower() == model.Name.ToLower());
                    if (isOrganizationCreated != null)
                    {
                        response.ReturnedId = isOrganizationCreated.Id;
                    }
                    else
                    {
                        var newOrganization = unitWork.OrganizationRepository.Insert(new EFOrganization()
                        {
                            OrganizationName = model.Name,
                        });
                        unitWork.Save();
                        response.ReturnedId = newOrganization.Id;
                    }
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse Update(int id, OrganizationModel organization)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var organizationObj = unitWork.OrganizationRepository.GetByID(id);
                IMessageHelper mHelper = new MessageHelper();

                if (organizationObj == null)
                {
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("Organization");
                    return response;
                }

                organizationObj.OrganizationName = organization.Name;
                unitWork.OrganizationRepository.Update(organizationObj);
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }

        public async Task<ActionResponse> MergeOrganizations(MergeOrganizationModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                IMessageHelper mHelper;
                ActionResponse response = new ActionResponse();

                var strategy = context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    // Achieving atomicity between original Catalog database operation and the
                    // IntegrationEventLog thanks to a local transaction
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var orgOne = unitWork.OrganizationRepository.GetByID(model.OrgFirst);
                        if (orgOne == null)
                        {
                            mHelper = new MessageHelper();
                            response.Message = mHelper.GetNotFound("One of merging organizations");
                            response.Success = false;
                            await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                        }

                        var orgTwo = unitWork.OrganizationRepository.GetByID(model.OrgSecond);
                        if (orgTwo == null)
                        {
                            mHelper = new MessageHelper();
                            response.Message = mHelper.GetNotFound("One of merging organizations");
                            response.Success = false;
                            await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                        }

                        var users = unitWork.UserRepository.GetManyQueryable(u => (u.OrganizationId.Equals(orgOne.Id) || u.OrganizationId.Equals(orgTwo.Id)));
                        var newOrganization = unitWork.OrganizationRepository.Insert(new EFOrganization()
                        {
                            OrganizationName = model.NewName,
                            RegisteredOn = DateTime.Now,
                            IsApproved = true
                        });
                        unitWork.Save();

                        foreach (var user in users)
                        {
                            user.Organization = newOrganization;
                            unitWork.UserRepository.Update(user);
                        }
                        unitWork.Save();

                        unitWork.OrganizationRepository.Delete(orgOne);
                        unitWork.OrganizationRepository.Delete(orgTwo);

                        unitWork.Save();
                        transaction.Commit();
                    }
                    return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
                });
                return await Task<ActionResponse>.Run(() => response).ConfigureAwait(false);
            }
        }

        public ActionResponse Approve(int id)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                IMessageHelper mHelper;
                var organization = unitWork.OrganizationRepository.GetByID(id);
                if (organization == null)
                {
                    mHelper = new MessageHelper();
                    response.Message = mHelper.GetNotFound("Organization");
                    response.Success = false;
                    return response;
                }

                try
                {
                    organization.IsApproved = true;
                    unitWork.OrganizationRepository.Update(organization);
                    unitWork.Save();
                }
                catch(Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }
    }
}
