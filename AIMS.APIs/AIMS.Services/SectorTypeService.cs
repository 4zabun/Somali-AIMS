﻿using AIMS.DAL.EF;
using AIMS.DAL.UnitOfWork;
using AIMS.Models;
using AIMS.Services.Helpers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Services
{
    public interface ISectorTypesService
    {
        /// <summary>
        /// Gets all sectorTypes
        /// </summary>
        /// <returns></returns>
        IEnumerable<SectorTypesView> GetAll();

        /// <summary>
        /// Gets all sectorTypes async
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SectorTypesView>> GetAllAsync();

        /// <summary>
        /// Adds a new section
        /// </summary>
        /// <returns>Response with success/failure details</returns>
        ActionResponse Add(SectorTypesModel sectorType);

        /// <summary>
        /// Updates a sectorType
        /// </summary>
        /// <param name="sectorType"></param>
        /// <returns></returns>
        ActionResponse Update(int id, SectorTypesModel sectorType);
    }

    public class SectorTypesService : ISectorTypesService
    {
        AIMSDbContext context;
        IMapper mapper;

        public SectorTypesService(AIMSDbContext cntxt, IMapper autoMapper)
        {
            context = cntxt;
            mapper = autoMapper;
        }

        public IEnumerable<SectorTypesView> GetAll()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorTypes = unitWork.SectorTypesRepository.GetAll();
                return mapper.Map<List<SectorTypesView>>(sectorTypes);
            }
        }

        public async Task<IEnumerable<SectorTypesView>> GetAllAsync()
        {
            using (var unitWork = new UnitOfWork(context))
            {
                var sectorTypes = await unitWork.SectorTypesRepository.GetAllAsync();
                return await Task<IEnumerable<SectorTypesView>>.Run(() => mapper.Map<List<SectorTypesView>>(sectorTypes)).ConfigureAwait(false);
            }
        }

        public ActionResponse Add(SectorTypesModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                try
                {
                    var newSectorTypes = unitWork.SectorTypesRepository.Insert(new EFSectorTypes() { TypeName = model.TypeName });
                    response.ReturnedId = newSectorTypes.Id;
                    unitWork.Save();
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public ActionResponse Update(int id, SectorTypesModel model)
        {
            using (var unitWork = new UnitOfWork(context))
            {
                ActionResponse response = new ActionResponse();
                var sectorTypeObj = unitWork.SectorTypesRepository.GetByID(id);
                if (sectorTypeObj == null)
                {
                    IMessageHelper mHelper = new MessageHelper();
                    response.Success = false;
                    response.Message = mHelper.GetNotFound("SectorTypes");
                    return response;
                }

                sectorTypeObj.TypeName = model.TypeName;
                unitWork.Save();
                response.Message = "1";
                return response;
            }
        }
    }
}