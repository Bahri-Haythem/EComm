﻿using API.Dtos;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace API.Helpers;

public class ProductUrlResolver : IValueResolver<Product, ProductDto, string>
{
    private readonly IConfiguration _configuration;

    public ProductUrlResolver(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
    {
        if (!string.IsNullOrEmpty(source.PictureUrl))
        {
            return _configuration["ApiUrl"] + source.PictureUrl;
        }

        return null;
    }
}
