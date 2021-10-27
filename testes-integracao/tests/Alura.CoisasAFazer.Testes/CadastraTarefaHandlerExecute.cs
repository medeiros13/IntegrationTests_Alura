using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Services.Handlers;
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

            var repo = new RepositorioFake();

            var handler = new CadastraTarefaHandler(repo);
            //act
            handler.Execute(comando);

            //assert
            var tarefa = repo.ObtemTarefas(tarefa => tarefa.Titulo == "Estudar Xunit").FirstOrDefault();
            Assert.NotNull(tarefa);
        }
    }
}
