﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using WordPressPCL.Models;
using WordPressPCL.Tests.Selfhosted.Utility;
using System.Linq;
using WordPressPCL.Utility;
using WordPressPCL.Models.Exceptions;
using System.Collections.Generic;

namespace WordPressPCL.Tests.Selfhosted;

[TestClass]
public class Pages_Tests
{
    private static WordPressClient _client;
    private static WordPressClient _clientAuth;

    [ClassInitialize]
    public static async Task Init(TestContext testContext)
    {
        _client = ClientHelper.GetWordPressClient();
        _clientAuth = await ClientHelper.GetAuthenticatedWordPressClient(testContext);
    }

    [TestMethod]
    public async Task Pages_Create()
    {
        Page page = new()
        {
            Title = new Title("Title 1"),
            Content = new Content("Content PostCreate")
        };
        Page createdPage = await _clientAuth.Pages.CreateAsync(page);

        Assert.AreEqual(page.Content.Raw, createdPage.Content.Raw);
        Assert.IsTrue(createdPage.Content.Rendered.Contains(page.Content.Rendered));
    }

    [TestMethod]
    public async Task Pages_Read()
    {
        List<Page> pages = await _client.Pages.QueryAsync(new PagesQueryBuilder());
        Assert.IsNotNull(pages);
        Assert.AreNotEqual(pages.Count, 0);
    }

    [TestMethod]
    public async Task Pages_Get()
    {
        List<Page> pages = await _client.Pages.GetAsync();
        Assert.IsNotNull(pages);
        Assert.AreNotEqual(pages.Count, 0);
    }

    [TestMethod]
    public async Task Pages_Update()
    {
        string testContent = $"Test {System.Guid.NewGuid()}";
        List<Page> pages = await _client.Pages.GetAllAsync();
        Assert.IsTrue(pages.Count > 0);

        Page page = pages.FirstOrDefault();
        page.Content.Raw = testContent;
        Page updatedPage = await _clientAuth.Pages.UpdateAsync(page);
        Assert.AreEqual(testContent, updatedPage.Content.Raw);
    }


    [TestMethod]
    public async Task Pages_Delete()
    {
        Page page = new()
        {
            Title = new Title("Title 1"),
            Content = new Content("Content PostCreate")
        };
        Page createdPage = await _clientAuth.Pages.CreateAsync(page);
        Assert.IsNotNull(createdPage);

        bool response = await _clientAuth.Pages.Delete(createdPage.Id);
        Assert.IsTrue(response);
        await Assert.ThrowsExceptionAsync<WPException>(async () =>
        {
            Page pageById = await _client.Pages.GetByIDAsync(createdPage.Id);
        });
    }

    [TestMethod]
    public async Task Pages_Query()
    {
        PagesQueryBuilder queryBuilder = new()
        {
            Page = 1,
            PerPage = 15,
            OrderBy = PagesOrderBy.Title,
            Order = Order.ASC,
            Statuses = new List<Status> { Status.Publish },
            Embed = true
        };
        List<Page> queryresult = await _client.Pages.QueryAsync(queryBuilder);
        Assert.AreEqual("?page=1&per_page=15&orderby=title&status=publish&order=asc&_embed=true&context=view", queryBuilder.BuildQuery());
        Assert.IsNotNull(queryresult);
        Assert.AreNotSame(queryresult.Count, 0);
    }
}
