using System;
using Moq;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using System.Linq;
using Xunit;
using ShoppingCart.Tests.Datasets;

namespace ShoppingCart.Tests
{
    public class CategoryControllerTests
    {
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<ICategoryRepository> repositoryMock;
        private ShoppingCart.Web.Areas.Admin.Controllers.CategoryController controller;

        public CategoryControllerTests()
        {
            repositoryMock = new Mock<ICategoryRepository>();
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Category).Returns(repositoryMock.Object);
            controller = new ShoppingCart.Web.Areas.Admin.Controllers.CategoryController(mockUnitOfWork.Object);
        }

        [Fact]
        public void CreateUpdate_WithNewCategory_CreatesCategory()
        {
            var categoryVM = new CategoryVM { Category = new Category { Id = 0, Name = "TestCategory" } };
            controller.CreateUpdate(categoryVM);
            repositoryMock.Verify(r => r.Add(It.IsAny<Category>()), Times.Once);
            mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void CreateUpdate_WithExistingCategory_UpdatesCategory()
        {
            var categoryVM = new CategoryVM { Category = new Category { Id = 1, Name = "TestCategory" } };
            controller.CreateUpdate(categoryVM);
            repositoryMock.Verify(r => r.Update(It.IsAny<Category>()), Times.Once);
            mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void DeleteData_WithNonExistingId_ThrowsException()
        {
            int nonExistingCategoryId = 999;
            Assert.Throws<Exception>(() => controller.DeleteData(nonExistingCategoryId));
        }
    }
}
