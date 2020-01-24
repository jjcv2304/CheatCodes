﻿
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int? ParentId { get; set; }
        public bool HasParent { get; set; }
        public bool HasChild { get; set; }
        public List<CategoryFieldValueDto> CategoryFieldValues { get; set; }        
        
    }
    public class CategoryCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ParentId { get; set; }
    }
    public class CategoryUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public List<CategoryFieldValueDto> CategoryFieldValues { get; set; }  
    }
    public class CategoryDeleteDto
    {
        public int Id { get; set; }
    }
    public class CategoryMoveUpDto
    {
        public int ParentId { get; set; }
        public int Id { get; set; }
    }
    public class CategoryMoveToSiblingDto
    {
        public int SiblingId { get; set; }
        public int CategoryId { get; set; }
    }
    
}