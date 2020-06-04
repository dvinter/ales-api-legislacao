using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dadosLegislacaoAPI.Models.DAO;
using dadosLegislacaoAPI.Models.Entidade;
using System.Web.Http;

namespace dadosLegislacaoAPI.Controller
{
    public class GetLegislacaoController : ApiController
    {

        [HttpGet]
        public object getDadosTipoAutoresLegislacao(string txPesquisa, string idLei)
        {
            switch (txPesquisa.ToUpper())
            {
                case "TIPOLEGISLACAO":
                    return new LegislacaoDAO().CarregarListaTipoLegislacao();
                case "LISTAAUTORES":
                    if (string.IsNullOrEmpty(idLei))
                        return new LegislacaoDAO().CarregarListaAutor();
                    else return new LegislacaoDAO().BuscarAutores(idLei);
                default:
                    return null;
            }
        }

        [HttpGet]
        public object getDadosLegislacao(String tema = "", String tipoNorma = "", String numeroNorma = "", String anoNorma = "", String dataInicialPesquisa = "", String dataFinalPesquisa = "", String autorNorma = "", String assuntoNorma = "", String situacao="", String strNumeroAnoProposicao = "")
        {
            return new LegislacaoDAO().pesquisarLegislacao(tipoNorma, numeroNorma, anoNorma, dataInicialPesquisa, dataFinalPesquisa, autorNorma, assuntoNorma, tema, situacao, strNumeroAnoProposicao);
        }

    }
}
