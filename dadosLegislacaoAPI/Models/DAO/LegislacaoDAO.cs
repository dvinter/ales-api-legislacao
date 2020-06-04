using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using dadosLegislacaoAPI.Models.Entidade;

namespace dadosLegislacaoAPI.Models.DAO
{
    public class LegislacaoDAO
    {
        /// <summary>
        /// Carrega os tipos de Normas Jurídica para lista no dropdown de pesquisa
        /// </summary>
        /// <returns></returns>
        public List<Legislacao> CarregarListaTipoLegislacao()
        {
            List<Legislacao> resultado = new List<Legislacao>();

            using (SqlConnection conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["conexaoPancas"].ConnectionString))
            {
                String sql =
                            " SELECT  " + Environment.NewLine +
                            "  cod_lei_tipo AS ORDEM_LEI  " + Environment.NewLine +
                            " ,let_nome AS TIPO_LEI  " + Environment.NewLine +
                            " FROM prl_leis_tipos   " + Environment.NewLine +
                            " WHERE let_www = 1   " + Environment.NewLine +
                            " ORDER BY ORDEM_LEI ";


                ////Consulta antiga buscando da intranet: SERVIDOR: VITORIA/ALES/SCNJ
                //String sql =
                //            " SELECT  " + Environment.NewLine +
                //            " ORDEM_LEI,  " + Environment.NewLine +
                //            " TIPO_LEI  " + Environment.NewLine +
                //            " FROM SNJ_TIPO_LEI   " + Environment.NewLine +
                //            " ORDER BY ORDEM_LEI ";




                SqlDataReader reader = null;

                try
                {

                    // Abre a conexão
                    conexao.Open();

                    // Passa a conexão para o objeto do comando
                    SqlCommand cmd = new SqlCommand(sql, conexao);

                    // Obtém a consulta com os resultados
                    using (reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                resultado.Add(new Legislacao()
                                {
                                    idTipoLegislacao = Convert.ToInt32(reader["ORDEM_LEI"]),
                                    txTipoLegislacao = reader["TIPO_LEI"].ToString()
                                });
                            }
                            catch (Exception e) { string erro = e.Message; }

                        }

                        reader.Close();
                    }
                    conexao.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (reader != null)
                        reader.Close();

                    if (conexao != null)
                        conexao.Close();
                }

            }

            return (resultado);

        }

        // ------------------------------------------------------------------------------------------

        /// <summary>
        /// Carrega os autores para lista no dropdown de pesquisa
        /// </summary>
        /// <returns></returns>
        public List<Legislacao> CarregarListaAutor()
        {
            List<Legislacao> resultado = new List<Legislacao>();

            using (SqlConnection conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["conexaoPancas"].ConnectionString))
            {

                SqlCommand cmd = conexao.CreateCommand();
                {

                    cmd.CommandText =

                        " SELECT DISTINCT  " + Environment.NewLine +

                        "      prl_leis_autores.cod_autor AS ID_AUTOR  " + Environment.NewLine +
                        " 	  ,neg_contatos.cod_contato  " + Environment.NewLine +
                        " 	  ,UPPER(neg_contatos.con_nome_razao) as AUTOR  " + Environment.NewLine +

                        "   FROM prl_leis_autores  " + Environment.NewLine +

                        "   INNER JOIN prl_autores ON  " + Environment.NewLine +
                        "   prl_autores.cod_autor = prl_leis_autores.cod_autor  " + Environment.NewLine +

                        "   INNER JOIN neg_contatos ON  " + Environment.NewLine +
                        "   neg_contatos.cod_contato = prl_autores.cod_contato  ";


                    ////Consulta antiga buscando da intranet: SERVIDOR: VITORIA/ALES/SCNJ
                    // cmd.CommandText =
                    //" SELECT     ID_AUTOR, AUTOR " +
                    //" FROM         SCNJ_AUTOR " +
                    //" ORDER BY AUTOR ";

                }

                conexao.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        resultado.Add(new Legislacao()
                        {
                            idAutor = reader["ID_AUTOR"].ToString(),
                            strAutor = reader["AUTOR"].ToString()
                        });
                    }
                    reader.Close();

                    resultado.Insert(0, new Legislacao()
                    {
                        idAutor = "0",
                        strAutor = "-- Selecione --"
                    });

                }
                conexao.Close();
            }
            return resultado;
        }

        // ------------------------------------------------------------------------------------------
        /// <summary>
        /// Busca Legislação de acordo com os parâmetros informados
        /// </summary>
        /// <param name="tipoNorma"></param>
        /// <param name="numeroNorma"></param>
        /// <param name="anoNorma"></param>
        /// <param name="dataInicialPesquisa"></param>
        /// <param name="dataFinalPesquisa"></param>
        /// <param name="autorNorma"></param>
        /// <param name="assuntoNorma"></param>
        /// <returns></returns>
        public List<Legislacao> pesquisarLegislacao(String tipoNorma, String numeroNorma, String anoNorma, String dataInicialPesquisa, String dataFinalPesquisa, String autorNorma, String assuntoNorma, String tema, String situacao, String strNumeroAnoProposicao)
        {
            List<Legislacao> resultado = new List<Legislacao>();

            using (SqlConnection conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["conexaoPancas"].ConnectionString))
            {
                SqlCommand cmd = conexao.CreateCommand();

                String sqlWhere = String.Empty;

                if (!String.IsNullOrEmpty(autorNorma) && (!autorNorma.Equals("0")))
                    sqlWhere += " AND prl_autores.cod_autor = " + "'" + autorNorma + "'" + Environment.NewLine;

                //sqlWhere += " AND (prl_proposicoes.cod_autor_principal_interno = " + "'" + idAutorProposicao + "'  OR pra.cod_autor = " + "'" + idAutorProposicao + "') " + Environment.NewLine;

                if (!String.IsNullOrEmpty(strNumeroAnoProposicao))
                {
                    sqlWhere += " AND (CONVERT(VARCHAR, prl_proposicoes.pro_numero) + '/' + CONVERT(VARCHAR, prl_proposicoes.pro_ano)) = '" + strNumeroAnoProposicao + "'" + Environment.NewLine;
                }

                if (!String.IsNullOrEmpty(tipoNorma) && (!tipoNorma.Equals("0")))
                    sqlWhere += " AND prl_leis.cod_lei_tipo = " + "'" + tipoNorma + "'" + Environment.NewLine;

                if (!String.IsNullOrEmpty(numeroNorma))
                    sqlWhere += " AND prl_leis.lei_numero = " + "'" + numeroNorma + "'" + Environment.NewLine;

                if (!String.IsNullOrEmpty(anoNorma))
                    sqlWhere += " AND (select year(lei_data)) = " + "'" + anoNorma + "'" + Environment.NewLine;
                if (!String.IsNullOrEmpty(tema))
                {
                    List<String> TemaLista = tema.Replace("[", "").Replace("]", "").Split(',').ToList();
                    sqlWhere += " AND ( 0 = 1 ";
                    for (int i=0;i<TemaLista.Count; i++)
                    {
                        sqlWhere += " OR temas.cod_tema = " + "'" + TemaLista[i] + "'" + Environment.NewLine;
                    }
                    sqlWhere += " ) ";
                }
                   
                if (!String.IsNullOrEmpty(situacao))
                    sqlWhere += " AND prl_leis_situacoes.cod_lei_situacao = " + "'" + situacao + "'" + Environment.NewLine;


                if (!String.IsNullOrEmpty(dataInicialPesquisa))
                {
                    DateTime dataInicial = Convert.ToDateTime(dataInicialPesquisa);
                    sqlWhere += " AND prl_leis.lei_data >= " + "'" + dataInicial + "'" + Environment.NewLine;
                }
                else
                {
                    sqlWhere += " AND prl_leis.lei_data >= " + "'01/01/1900 00:00:00'" + Environment.NewLine;
                }

                if (!String.IsNullOrEmpty(dataFinalPesquisa))
                {
                    DateTime dataFinal = Convert.ToDateTime(dataFinalPesquisa);
                    sqlWhere += " AND prl_leis.lei_data <= " + "'" + dataFinal + "'" + Environment.NewLine;
                }
                else
                {
                    sqlWhere += " AND prl_leis.lei_data <= " + "'" + DateTime.Now.ToString("dd/MM/yyyy 23:59:00") + "'" + Environment.NewLine;
                }

                if (!String.IsNullOrEmpty(assuntoNorma))
                {
                    //sqlWhere += " AND prl_leis.lei_ementa like '%" + assuntoNorma + "%' COLLATE Latin1_General_CI_AI " + Environment.NewLine;

                    //Pesquisa utilizando o SQL Server Full-Text Indexing
                    String[] allPhrases = assuntoNorma.Split(',');
                    for(int i = 0; i< allPhrases.Length; i++)
                    {
                        sqlWhere += " AND CONTAINS(lei_ementa, '" + Util.criarParametroContains(allPhrases[i]) + "') " + Environment.NewLine;
                    }
                    
                }


                cmd.CommandText =


                        " SELECT DISTINCT  " + Environment.NewLine +
                        "      prl_leis.cod_lei_tipo " + Environment.NewLine +
                        "     ,prl_leis.cod_lei " + Environment.NewLine +
                        " 	  ,prl_leis_tipos.let_nome as tipo_lei " + Environment.NewLine +
                        "     ,lei_numero as numero_lei	    " + Environment.NewLine +
                        " 	  ,(select year(lei_data)) as numero_ano_lei " + Environment.NewLine +
                        //" 	  ,prl_autores.cod_autor " + Environment.NewLine +
                        " 	  ,neg_contatos.con_apelido_fantasia as autor " + Environment.NewLine +
                        "     ,lei_ementa as ementa_lei " + Environment.NewLine +
                        "     ,lei_data as data_publicacao " + Environment.NewLine +
                        "     ,lei_arquivo as nm_pdf " + Environment.NewLine +
                        "     ,lei_html as nm_html " + Environment.NewLine +
                        " 	  ,lei_compilada as nm_html_compilado " + Environment.NewLine +
                        " 	  ,CONVERT(VARCHAR, prl_proposicoes.pro_numero) + '/' + CONVERT(VARCHAR, prl_proposicoes.pro_ano) AS numeroAnoProposicao " + Environment.NewLine +
                        " 	  ,prl_leis.cod_proposicao " + Environment.NewLine +
                        " 	  ,prl_proposicoes.pro_numero as numeroProposicao " + Environment.NewLine +
                        " 	  ,prl_proposicoes.pro_ano as anoProposicao " + Environment.NewLine +
                        " 	  ,prl_proposicoes.cod_proposicao_tipo " + Environment.NewLine +
                        " 	  ,prl_leis.cod_lei_situacao " + Environment.NewLine +
                        " 	  ,prl_leis_situacoes.les_nome " + Environment.NewLine +
                        "     ,prl_leis.cod_lei_origem " + Environment.NewLine +
                        "     ,lei_situacao_origem " + Environment.NewLine +

                        "   FROM agape.dbo.prl_leis " + Environment.NewLine +

                        // inicio da query adicionada para pegar tema 

                        " LEFT JOIN leis_temas ON " + Environment.NewLine +
                        " prl_leis.cod_lei = leis_temas.cod_lei " + Environment.NewLine +

                        " LEFT JOIN temas ON " + Environment.NewLine +
                        " leis_temas.cod_tema = temas.cod_tema " + Environment.NewLine +

                        // fim da query

                        "   INNER JOIN prl_leis_tipos ON " + Environment.NewLine +
                        "   prl_leis_tipos.cod_lei_tipo = prl_leis.cod_lei_tipo " + Environment.NewLine +

                        "   LEFT OUTER JOIN prl_leis_autores ON " + Environment.NewLine +
                        "   prl_leis_autores.cod_lei = prl_leis.cod_lei " + Environment.NewLine +

                        "   LEFT OUTER JOIN prl_autores ON " + Environment.NewLine +
                        "   prl_autores.cod_autor = prl_leis_autores.cod_autor " + Environment.NewLine +

                        "   LEFT OUTER JOIN neg_contatos ON " + Environment.NewLine +
                        "   neg_contatos.cod_contato = prl_autores.cod_contato " + Environment.NewLine +

                        "   LEFT OUTER JOIN prl_proposicoes ON " + Environment.NewLine +
                        "   prl_proposicoes.cod_proposicao = prl_leis.cod_proposicao " + Environment.NewLine +

                        "   LEFT OUTER JOIN prl_leis_situacoes ON " + Environment.NewLine +
                        "   prl_leis_situacoes.cod_lei_situacao = prl_leis.cod_lei_situacao " + Environment.NewLine +

                        "   WHERE 0 = 0 " + Environment.NewLine +
                            sqlWhere + Environment.NewLine +

                        " ORDER BY prl_leis_tipos.let_nome, lei_data DESC, lei_numero DESC  ";









                ////Consulta antiga buscando da intranet: SERVIDOR: VITORIA/ALES/SCNJ

                //                                    " SELECT SCNJ_TIPO_LEI.Tipo_Lei, SCNJ_LEI.NUMERO_LEI, SCNJ_LEI.NUMERO_ANO_LEI, SCNJ_AUTOR.AUTOR, SCNJ_LEI.EMENTA_LEI, SCNJ_LEI.ASSUNTO_LEI, " + Environment.NewLine +
                //                                    " SCNJ_LEI.NUMERO_PROPOSICAO, SCNJ_LEI.DATA_PUBLICACAO, SCNJ_LEI.NM_HTML,SCNJ_LEI.NM_HTML_COMPILADO, CONVERT(VARCHAR, TAB01.numeroProposicao) " + Environment.NewLine +
                //                                    " + '/' + CONVERT(VARCHAR, TAB01.anoProposicao) AS PROPOSICAO, " + Environment.NewLine +
                //                                    " TAB01.numeroProposicao, TAB01.anoProposicao, TAB01.codTipoProposicao " + Environment.NewLine +
                //                                    " FROM         SCNJ_TIPO_LEI INNER JOIN " + Environment.NewLine +
                //                                    " SCNJ_LEI ON SCNJ_TIPO_LEI.Tipo_Lei = SCNJ_LEI.TIPO_LEI " + Environment.NewLine +
                //                                    " LEFT OUTER JOIN " + Environment.NewLine +

                //" OPENROWSET('SQLOLEDB', 'pancas'; 'bdados'; 'usu@rio',  " + Environment.NewLine +
                //"                       'SELECT      " + Environment.NewLine +
                //" prl_proposicoes.cod_proposicao as idProposicao,  " + Environment.NewLine +
                //" prl_proposicoes.pro_numero     as numeroProposicao,  " + Environment.NewLine +
                //" prl_proposicoes.pro_ano        as anoProposicao,  " + Environment.NewLine +
                //" prl_proposicoes_tipos.prt_nome as tipoProposicao, " + Environment.NewLine +
                //" prl_proposicoes.pro_ementa     as ementa,  " + Environment.NewLine +
                //" autorProposicao = (select neg_contatos.con_nome_razao  " + Environment.NewLine +
                //"                                   from prl_autores  " + Environment.NewLine +
                //"                                   INNER JOIN neg_contatos  " + Environment.NewLine +
                //"                                   ON prl_autores.cod_contato = neg_contatos.cod_contato " + Environment.NewLine +
                //"                                   WHERE prl_proposicoes.cod_autor_principal_interno = prl_autores.cod_autor), " + Environment.NewLine +
                //" prl_proposicoes.cod_proposicao_tipo   as codTipoProposicao " + Environment.NewLine +
                //" FROM          " + Environment.NewLine +
                //" prl_proposicoes " + Environment.NewLine +
                //" INNER JOIN prl_proposicoes_tipos  " + Environment.NewLine +
                //"  ON prl_proposicoes.cod_proposicao_tipo = prl_proposicoes_tipos.cod_proposicao_tipo " + Environment.NewLine +
                //" order by " + Environment.NewLine +
                //" prl_proposicoes.pro_ano desc, " + Environment.NewLine +
                //" prl_proposicoes.pro_numero asc, " + Environment.NewLine +
                //" prl_proposicoes_tipos.prt_nome') AS TAB01 " + Environment.NewLine +

                //" ON  " + Environment.NewLine +
                //"  SCNJ_LEI.ID_PROPOSICAO = TAB01.idProposicao  " + Environment.NewLine +


                //                                    " LEFT OUTER JOIN  SCNJ_LEI_AUTOR ON SCNJ_LEI.ID_LEI = SCNJ_LEI_AUTOR.ID_LEI LEFT OUTER JOIN " + Environment.NewLine +
                //                                    " SCNJ_AUTOR ON SCNJ_AUTOR.ID_AUTOR = SCNJ_LEI_AUTOR.ID_AUTOR " + Environment.NewLine +
                //                                    " WHERE 0 = 0 " + Environment.NewLine +
                //                                      sqlWhere + Environment.NewLine +
                //                                    " ORDER BY SCNJ_LEI.TIPO_LEI, SCNJ_LEI.DATA_PUBLICACAO DESC, SCNJ_LEI.NUMERO_LEI DESC ";

                // }



                conexao.Open();
                cmd.CommandTimeout = 1000;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {

                            resultado.Add(new Legislacao()
                            {
                                txTipoLegislacao = reader["tipo_lei"].ToString(),
                                strNumero = reader["numero_lei"].ToString(),
                                strAno = reader["numero_ano_lei"].ToString(),
                                //strAutor = reader["autor"].ToString(),
                                strEmenta = reader["ementa_lei"].ToString(),
                                strNrProposicao = reader["numeroProposicao"].ToString(),
                                strAnoProposicao = reader["anoProposicao"].ToString(),
                                strNumeroAnoProposicao = reader["numeroAnoProposicao"].ToString(),

                                //Conversão de data nula do banco - IsDBNull(x) onde x é o índice da posição do campo na consulta. 
                                idTipoProposicao = (Int32?)(reader.IsDBNull(13) ? null : reader["cod_proposicao_tipo"]),
                                idNormaLegislativa = reader["cod_lei"].ToString(),
                                strAnexoPdf = reader["nm_pdf"].ToString(),
                                strAnexoHtml = reader["nm_html"].ToString(),
                                Data = Convert.ToDateTime(reader["data_publicacao"]),
                                strAnexoCompilado = reader["NM_HTML_COMPILADO"].ToString(),
                                strAutor = reader["autor"].ToString()

                            });
                        }
                        catch (Exception e) { string erro = e.Message; }

                    }
                    reader.Close();
                }

                conexao.Close();
            }
            return resultado;
        }

        // ------------------------------------------------------------------------------------------




        /// <summary>
        /// Retorna todos os autores relacionados com uma norma específica
        /// </summary>
        /// <param name="idLei"></param>
        /// <returns></returns>
        public List<Legislacao> BuscarAutores(String idLei)
        {
            List<Legislacao> resultado = new List<Legislacao>();

            using (SqlConnection conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["conexaoPancas"].ConnectionString))
            {
                SqlCommand cmd = conexao.CreateCommand();
                {
                    String sqlWhere = String.Empty;

                    cmd.CommandText =

                    "   SELECT prl_leis.cod_lei, prl_autores.cod_autor, neg_contatos.con_nome_razao  " + Environment.NewLine +
                    " 	FROM  prl_leis   " + Environment.NewLine +
                    "	INNER JOIN prl_leis_autores ON prl_leis_autores.cod_lei = prl_leis.cod_lei  " + Environment.NewLine +
                    " 	INNER JOIN prl_autores ON prl_autores.cod_autor = prl_leis_autores.cod_autor  " + Environment.NewLine +
                    " 	INNER JOIN neg_contatos ON prl_autores.cod_contato = neg_contatos.cod_contato    " + Environment.NewLine +
                    " 	where prl_leis.cod_lei = " + idLei;

                }
                conexao.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            resultado.Add(new Legislacao()
                            {
                                idNormaLegislativa = reader["cod_lei"].ToString(),
                                idAutor = reader["cod_autor"].ToString(),
                                strAutor = reader["con_nome_razao"].ToString()

                            });
                        }
                        catch (Exception e) { string erro = e.Message; }

                    }
                    reader.Close();
                }

                conexao.Close();
            }
            return resultado;
        }


    }
}