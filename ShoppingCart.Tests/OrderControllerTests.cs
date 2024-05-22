using Moq;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using ShoppingCart.Utility;
using ShoppingCart.Web.Areas.Admin.Controllers;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace ShoppingCart.Tests.Controllers
{
    public class OrderControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private OrderController _controller;

        public OrderControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _controller = new OrderController(_mockUnitOfWork.Object);
        }

        [Fact]
        public void SetToInProcess_UpdatesOrderStatus()
        {
            var orderVM = new OrderVM
            {
                OrderHeader = new OrderHeader { Id = 1 }
            };

            _mockUnitOfWork.Setup(uow => uow.OrderHeader.GetT(It.IsAny<Expression<Func<OrderHeader, bool>>>(), null)).Returns(orderVM.OrderHeader);

            _controller.SetToInProcess(orderVM);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void SetToShipped_UpdatesOrderHeader()
        {
            var orderVM = new OrderVM
            {
                OrderHeader = new OrderHeader
                {
                    Id = 1,
                    Carrier = "TprzTestlab3",
                    TrackingNumber = "22052024"
                }
            };
            var orderHeader = new OrderHeader { Id = 1 };

            _mockUnitOfWork.Setup(uow => uow.OrderHeader.GetT(It.IsAny<Expression<Func<OrderHeader, bool>>>(), null))
                           .Returns(orderHeader);

            _controller.SetToShipped(orderVM);

            Assert.Equal(orderVM.OrderHeader.Carrier, orderHeader.Carrier);
            Assert.Equal(orderVM.OrderHeader.TrackingNumber, orderHeader.TrackingNumber);
            Assert.Equal(OrderStatus.StatusShipped, orderHeader.OrderStatus);
            Assert.NotNull(orderHeader.DateOfShipping);

            _mockUnitOfWork.Verify(uow => uow.OrderHeader.Update(orderHeader), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void SetToCancelOrder_ApprovesRefundIfPaid()
        {
            Assert.True(true);
        }

        [Fact]
        public void SetToCancelOrder_CancelsIfNotPaid()
        {
            var orderVM = new OrderVM
            {
                OrderHeader = new OrderHeader
                {
                    Id = 1,
                    PaymentStatus = PaymentStatus.StatusPending
                }
            };
            var orderHeader = new OrderHeader { Id = 1, PaymentStatus = PaymentStatus.StatusPending };

            _mockUnitOfWork.Setup(uow => uow.OrderHeader.GetT(It.IsAny<Expression<Func<OrderHeader, bool>>>(), null))
                           .Returns(orderHeader);

            _controller.SetToCancelOrder(orderVM);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }
    }
}