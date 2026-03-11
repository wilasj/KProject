using KProject.Application.Clientes.CriaCliente;
using KProject.Application.Interfaces;
using KProject.Application.Interfaces.Clientes;
using KProject.Common;
using KProject.Domain.Clientes;
using NSubstitute;
using Shouldly;

namespace KProject.Tests.Unit.Clientes;

public class CriaClienteCommandHandlerTests
{
    private readonly IClienteRepository _clientes = Substitute.For<IClienteRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private Task<Result<int>> Handle(CriaClienteCommand command) =>
        new CriaClienteCommandHandler(_clientes, _unitOfWork)
            .Handle(command, TestContext.Current.CancellationToken);

    [Fact]
    public async Task CriaCliente_Valido_PersistERetornaId()
    {
        var result = await Handle(new CriaClienteCommand { Nome = "Hospital Silva" });

        result.IsSuccess.ShouldBeTrue();
        await _clientes.Received(1).AddAsync(Arg.Any<Cliente>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
