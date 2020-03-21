﻿using Application.RabbitMQ;
using Application.Utils;
using Application.Utils.Interfaces;
using CSharpFunctionalExtensions;

namespace Application
{
    public sealed class CategoryDeleteCommand : ICommand
    {
        public int Id { get; set; }

        public CategoryDeleteCommand(int id)
        {
            Id = id;
        }

        internal sealed class CategoryDeleteCommandHandler : ICommandHandler<CategoryDeleteCommand>
        {
            private readonly IUnitOfWork _unitOfWork;

            public CategoryDeleteCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Result Handle(CategoryDeleteCommand categoryDeleteCommand)
            {
                var categoryRepository = _unitOfWork.CategoryCommandRepository;
                var category = MapService.Map(categoryDeleteCommand);
                categoryRepository.Delete(category);
                SendNotification(categoryDeleteCommand);

                _unitOfWork.Commit();
                return Result.Ok();
            }
            private void SendNotification(CategoryDeleteCommand categoryDeleteCommand)
            {
              RabbitMQClient client = new RabbitMQClient();
              client.DeleteCategory(categoryDeleteCommand);
              client.Close();
            }
        }
    }
}
