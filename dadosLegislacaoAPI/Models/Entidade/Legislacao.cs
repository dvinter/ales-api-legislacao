using System;
using System.ComponentModel.DataAnnotations;

namespace dadosLegislacaoAPI.Models.Entidade
{
    public class Legislacao
    {
        
        public string idNormaLegislativa { get; set; } //Código da norma (Lei, ato, etc)

        public Int32 idTipoLegislacao { get; set; }

        [Display(Name = "Data")]
        public System.DateTime Data { get; set; }

        [Display(Name = "Autor")]
        public string strAutor { get; set; }

        public String idAutor { get; set; }

        [Display(Name = "Espécie normativa")]
        public string txTipoLegislacao { get; set; }

        [Display(Name = "Assunto")]
        public string strAssunto { get; set; }

        [Display(Name = "Nº da norma")]
        public string strNumero { get; set; }

        [Display(Name = "Ano")]
        public string strAno { get; set; }

        [Display(Name = "Ementa")]
        public string strEmenta { get; set; }

        [Display(Name = "Nº da proposição")]
        public string strNrProposicao { get; set; }

        public string strAnoProposicao { get; set; }

        public string situacao { get; set; }

        public string tema { get; set; }

        public string strNumeroAnoProposicao { get; set; }

        public Nullable<Int32> idTipoProposicao { get; set; }

        public string strAnexoPdf { get; set; }

        public string strAnexoHtml { get; set; }

        public string strAnexoCompilado { get; set; }

    }
}
