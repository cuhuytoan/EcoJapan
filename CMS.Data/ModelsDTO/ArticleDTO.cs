using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CMS.Data.ModelsDTO
{
    public class ArticleDTO
    {
        public int? Id { get; set; }
        public int? ArticleTypeId { get; set; }
        public string ArticleCategoryIds { get; set; }
        public int? ProductBrandId { get; set; }
        public int? ArticleStatusId { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập tên bài viết")]
        public string Name { get; set; }
        public string SubTitle { get; set; }            
        public string Image { get; set; }
        public string ImageDescription { get; set; }
        public string BannerImage { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Content { get; set; }
        public string Author { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc")]
        public DateTime? EndDate { get; set; }
        public bool? Active { get; set; }
        public int? Counter { get; set; }
        public string CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string LastEditBy { get; set; }
        public DateTime? LastEditDate { get; set; }
        public string Url { get; set; }
        public string Tags { get; set; }
        public bool? CanCopy { get; set; }
        public bool? CanComment { get; set; }
        public bool? CanDelete { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
    }

    public class BreadCrum
    {
        public string Url { get; set; }
        public string BreadCrumName { get; set; }
    }
}
