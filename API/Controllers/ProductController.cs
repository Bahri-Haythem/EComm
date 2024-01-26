using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Specifications;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace API.Controllers;

public class ProductController : ControllerBase
{
    public static void AddRoutes(WebApplication app)
    {
        app.MapPost("/api/Product", async (
           [FromBody] ProductSpecParams productParams,
           [FromServices] IGenericRepository<Product> _productRepo,
           [FromServices] IMapper _mapper) =>
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);

            var countSpec = new ProductWithFiltersForCountSpecification(productParams);

            var total = await _productRepo.CountAsync(countSpec);

            var products = await _productRepo.GetListAsync(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDto>>(products);

            var page = new Pagination<ProductDto>(productParams.PageIndex, productParams.PageSize, total, data);

            return page;
        }).WithTags("Product");
        app.MapPost("/api/Product/{id}", handler: async (
            int id,
            [FromServices] IGenericRepository<Product> _productRepo,
            [FromServices] IMapper _mapper) =>
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            var product = await _productRepo.GetEntityWithSpecAsync(spec);

            if (product == null) return Results.NotFound(new ApiResponse(404));


            return Results.Ok(_mapper.Map<Product, ProductDto>(product));
        }).WithTags("Product");
        app.MapGet("/api/Product/Brand", async (
            [FromServices] IGenericRepository<ProductBrand> _brandRepo) =>
            {
                var res = await _brandRepo.GetAllAsync();
                return res;
            }
        ).WithTags("Product");
        app.MapGet("/api/Product/Type", async (
            [FromServices] IGenericRepository<ProductType> _typeRepo) =>
         Results.Ok(await _typeRepo.GetAllAsync())
        ).WithTags("Product");
    }
}
