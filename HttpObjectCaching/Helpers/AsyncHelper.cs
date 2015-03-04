﻿
// <copyright file="AsyncHelper.cs" company="University of Cincinnati">
// Copyright (c) 2015. All rights reserved.
// </copyright>
// <author>Bjorg Prodan</author>
// <summary>
// The AsyncHelper.cs source file. Created: 02/20/2015 1:22 PM
// Last modified: 02/20/2015 1:22 PM
// </summary> 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace HttpObjectCaching.Helpers
{
    public static class AsyncHelper
    {
        private static readonly TaskFactory MyTaskFactory = new TaskFactory(
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            var ctx = HttpContext.Current;

            return MyTaskFactory.StartNew(() =>
            {
                HttpContext.Current = ctx;
                return func();
            }).Unwrap().GetAwaiter().GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            var ctx = HttpContext.Current;
            MyTaskFactory.StartNew(() =>
            {
                HttpContext.Current = ctx;
                return func();
            }).Unwrap().GetAwaiter().GetResult();
        }
        public static async Task<TResult> RunAsync<TResult>(Func<TResult> func)
        {
            var ctx = HttpContext.Current;

            return await MyTaskFactory.StartNew(() =>
            {
                HttpContext.Current = ctx;
                return func();
            });
        }

        
        
        
        public static async Task<IEnumerable<T>> WhereAsync<T>(this IEnumerable<T> target, Func<T, Task<bool>> predicateAsync)
        {
            var tasks = target.Select(async x => new { Predicate = await predicateAsync(x), Value = x }).ToArray();
            var results = await Task.WhenAll(tasks);

            return results.Where(x => x.Predicate).Select(x => x.Value);
        }

        public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this IEnumerable<TSource> values, Func<TSource, Task<TResult>> asyncSelector)
        {
            return await Task.WhenAll(values.Select(asyncSelector));
        }

        public static Task WhenAllAsync<T>(this IEnumerable<T> values, Func<T, Task> asyncAction)
        {
            return Task.WhenAll(values.Select(asyncAction));
        }


        public static Task ForEachAsync<T>(this List<T> values, Func<T, Task> asyncAction)
        {
            return Task.WhenAll(values.Select(asyncAction));
        }
        public static async Task<List<T>> WhereAsync<T>(this List<T> target, Func<T, Task<bool>> predicateAsync)
        {
            var tasks = target.Select(async x => new { Predicate = await predicateAsync(x), Value = x }).ToArray();
            var results = await Task.WhenAll(tasks);

            return results.Where(x => x.Predicate).Select(x => x.Value).ToList();
        }

        public static async Task<List<TResult>> SelectAsync<TSource, TResult>(this List<TSource> values, Func<TSource, Task<TResult>> asyncSelector)
        {
            return (await Task.WhenAll(values.Select(asyncSelector))).ToList();
        }

        public static Task WhenAllAsync<T>(this List<T> values, Func<T, Task> asyncAction)
        {
            return Task.WhenAll(values.Select(asyncAction));
        }
    }

}
