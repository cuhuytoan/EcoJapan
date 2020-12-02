using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CMS.Data.DataEntity;
using CMS.Data.ModelsDTO;
using CMS.Data.ModelsStore;

namespace CMS.Website.AutoMap
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Account
            CreateMap<AspNetUsers, AspNetUsersDTO>().ReverseMap();
            CreateMap<AspNetUserProfiles, AspNetUserProfilesDTO>().ReverseMap();
            CreateMap<AspNetUserRoles, AspNetUserRolesDTO>().ReverseMap();
            CreateMap<AspNetUserInfo, AspNetUserInfoDTO>().ReverseMap();
            //Article
            CreateMap<Article, ArticleDTO>().ReverseMap();
            CreateMap<ArticleCategory, ArticleCategoryDTO>().ReverseMap();

            CreateMap<ArticleSearchDTO, ArticleSearch_Result>().ReverseMap();

            CreateMap<ArticleGetByBlockIdDTO, ArticleGetByBlockId_Result>().ReverseMap();
            CreateMap<ArticleGetTopByCategoryIdDTO, ArticleGetTopByCategoryId_Result>().ReverseMap();
            CreateMap<ArticleGetByBlockIdDTO, ArticleSearch_Result>().ReverseMap();
            CreateMap<ArticleGetByBlockIdDTO, ArticleGetTopByCategoryId_Result>().ReverseMap();
            CreateMap<ArticleGetByBlockIdDTO, ArticleGetNewByCategoryId_Result>().ReverseMap();
            CreateMap<ArticleSearchDTO, ArticleGetNewByCategoryId_Result>().ReverseMap();
            CreateMap<Article, ArticleSearch_Result>().ReverseMap();
            CreateMap<ArticleGetByBlockIdDTO, ArticleGetByCategoryId_Result>().ReverseMap();
            CreateMap<ArticleDTO, ArticleSearch_Result>().ReverseMap();

        }
    }
}
