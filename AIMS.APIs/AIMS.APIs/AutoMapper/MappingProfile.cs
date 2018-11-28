﻿using AIMS.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIMS.APIs.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EFSector, SectorView>().ReverseMap();

            CreateMap<EFOrganizationTypes, OrganizationTypeView>();

            CreateMap<EFUser, UserView>()
                .ForMember(u => u.Organization, opts => opts.MapFrom(source => source.Organization.OrganizationName))
                .ForMember(u => u.OrganizationId, opts => opts.MapFrom(source => source.Organization.Id));

            CreateMap<EFOrganization, OrganizationView>()
                .ForMember(o => o.TypeName, opts => opts.MapFrom(source => source.OrganizationType.TypeName));

            CreateMap<EFOrganization, OrganizationViewModel>()
                .ForMember(o => o.OrganizationTypeId, opts => opts.MapFrom(source => source.OrganizationType.Id));

            CreateMap<EFUserNotifications, NotificationView>()
                .ForMember(n => n.Dated, opts => opts.MapFrom(source => source.Dated.ToShortDateString()));

            CreateMap<EFLocation, LocationView>().ReverseMap();

            CreateMap<EFSectorCategory, SectorCategoryView>()
                .ForMember(c => c.SectorType, opts => opts.MapFrom(source => source.SectorType.TypeName));

            CreateMap<EFSectorCategory, SectorCategoryViewModel>()
                .ForMember(s => s.SectorTypeId, opts => opts.MapFrom(source => source.SectorType.Id));

            CreateMap<EFSectorSubCategory, SectorSubCategoryView>()
                .ForMember(s => s.SectorCategory, opts => opts.MapFrom(source => source.SectorCategory.Category));

            CreateMap<EFSectorSubCategory, SectorSubCategoryViewModel>()
                .ForMember(s => s.CategoryId, opts => opts.MapFrom(source => source.SectorCategory.Id));
        }
    }
}
