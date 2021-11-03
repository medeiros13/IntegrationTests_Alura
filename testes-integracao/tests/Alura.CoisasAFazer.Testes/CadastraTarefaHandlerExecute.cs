using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Alura.CoisasAFazer.Testes
{
    public class CadastraTarefaHandlerExecute
    {
        [Fact]
        public void DadaTarefaComInformacoesValidasDeveIncluirNoDB()
        {
            //arrange
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2021, 12, 31));

            var mock = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("TarefasComInformacoesValidasContext")
                .Options;
            var context = new DbTarefasContext(options);

            var repo = new RepositorioTarefa(context);
            var handler = new CadastraTarefaHandler(repo, mock.Object);

            //act
            handler.Execute(comando);

            //assert
            var tarefa = repo.ObtemTarefas(tarefa => tarefa.Titulo == "Estudar Xunit").FirstOrDefault();
            Assert.NotNull(tarefa);
        }

        [Fact]
        public void QuandoExceptionForLancadaDeveResultadoIsSuccessDeveSerFalso()
        {
            //arrange
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2021, 12, 31));

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var mock = new Mock<IRepositorioTarefas>();

            //Mock configura que, sempre que o m�todo IncluirTarefas for chamado, para qualquer argumento de entrada do tipo tarefas[], ir� lan�ar a exce��o "Houve um erro na inclus�o de tarefas".
            mock.Setup(repositorio => repositorio.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(new Exception("Houve um erro na inclus�o de tarefas"));

            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo, mockLogger.Object);

            //act
            CommandResult resultado = handler.Execute(comando);

            //assert
            Assert.False(resultado.IsSuccess);
        }

        [Fact]
        public void QuandoExceptionForLancadaDeveLogarAMensagemDaExcecao()
        {
            //arrange
            var mensagemDeErroEsperada = "Houve um erro na inclus�o de tarefas";

            var excecaoEsperada = new Exception(mensagemDeErroEsperada);

            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2021, 12, 31));

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var mock = new Mock<IRepositorioTarefas>();

            //Mock configura que, sempre que o m�todo IncluirTarefas for chamado, para qualquer argumento de entrada do tipo tarefas[], ir� lan�ar a exce��o "Houve um erro na inclus�o de tarefas".
            mock.Setup(repositorio => repositorio.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(excecaoEsperada);

            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo, mockLogger.Object);

            //act
            CommandResult resultado = handler.Execute(comando);

            //assert
            mockLogger.Verify(log =>
                log.Log(
                    LogLevel.Error, //N�vel de log => logError
                    It.IsAny<EventId>(), //identificador do evento
                    It.IsAny<object>(), //objeto que ser� logado
                    excecaoEsperada, //exce��o que ser� logada
                    It.IsAny<Func<object, Exception, string>>() //fun��o que converte o objeto e a exce��o em uma string
                ),
                Times.Once());

        }
    }
}
