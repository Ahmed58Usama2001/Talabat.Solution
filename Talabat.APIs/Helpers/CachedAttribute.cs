﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Helpers;

public class CachedAttribute : Attribute, IAsyncActionFilter
{
    private readonly int _timeToLiveInSeconds;

    public CachedAttribute(int timeToLiveInSeconds)
    {
        _timeToLiveInSeconds = timeToLiveInSeconds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
        // Ask CLR to create object from responsecacheservice explicitly

        var cacheKey=GenerateCacheKeyFromRequest(context.HttpContext.Request);

        var response =await responseCacheService.GetCachedResponseAsync(cacheKey);

        if(!string.IsNullOrEmpty(cacheKey))
        {
            var result = new ContentResult()
            {
                Content=response,
                ContentType="application/json",
                StatusCode=200
            };

            context.Result=result;
            return;
        }

        var executedActionContext = await next.Invoke();

        if(executedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null) 
        {
            await responseCacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
        }
    }

    private string GenerateCacheKeyFromRequest(HttpRequest request)
    {
        var keyBuilder = new StringBuilder();

        keyBuilder.Append(request.Path);

        foreach (var (key,value) in request.Query.OrderBy(r=>r.Key))
        {
            keyBuilder.Append($"|{key}-{value}");
        }

        return keyBuilder.ToString();
    }
}
