﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordPressPCL.Interfaces;
using WordPressPCL.Models;
using WordPressPCL.Utility;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace WordPressPCL.Client
{
    public class Categories : ICRUDOperationAsync<Category>
    {
        #region Init
        private string _defaultPath;
        private const string _methodPath = "categories";
        private HttpHelper _httpHelper;
        public Categories(ref HttpHelper HttpHelper, string defaultPath)
        {
            _defaultPath = defaultPath;
            _httpHelper = HttpHelper;
        }
        #endregion
        #region Interface Realisation
        public async Task<Category> Create(Category Entity)
        {
            var postBody = new StringContent(JsonConvert.SerializeObject(Entity).ToString(), Encoding.UTF8, "application/json");
            return (await _httpHelper.PostRequest<Category>($"{_defaultPath}{_methodPath}", postBody)).Item1;
        }

        public async Task<Category> Update(Category Entity)
        {
            var postBody = new StringContent(JsonConvert.SerializeObject(Entity).ToString(), Encoding.UTF8, "application/json");
            return (await _httpHelper.PostRequest<Category>($"{_defaultPath}{_methodPath}/{Entity.Id}", postBody)).Item1;
        }

        public async Task<HttpResponseMessage> Delete(int ID)
        {
            return await _httpHelper.DeleteRequest($"{_defaultPath}{_methodPath}/{ID}").ConfigureAwait(false);
        }

        public async Task<IEnumerable<Category>> GetAll(bool embed = false)
        {
            //100 - Max posts per page in WordPress REST API, so this is hack with multiple requests
            List<Category> categories = new List<Category>();
            List<Category> categories_page = new List<Category>();
            int page = 1;
            do
            {
                categories_page = (await _httpHelper.GetRequest<IEnumerable<Category>>($"{_defaultPath}{_methodPath}?per_page=100&page={page++}", embed).ConfigureAwait(false))?.ToList<Category>();
                if (categories_page != null) { categories.AddRange(categories_page); }

            } while (categories_page != null);

            return categories;
            //return await _httpHelper.GetRequest<IEnumerable<Category>>($"{_defaultPath}{_methodPath}", embed).ConfigureAwait(false);
        }

        public async Task<Category> GetByID(int ID, bool embed = false)
        {
            return await _httpHelper.GetRequest<Category>($"{_defaultPath}{_methodPath}/{ID}", embed).ConfigureAwait(false);
        }

        #endregion

        #region Custom

        #endregion
    }
}
