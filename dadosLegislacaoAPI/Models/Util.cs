using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Net;
using System.Net.Sockets;

namespace dadosLegislacaoAPI.Models
{
    public static class Util
    {

        public static String RemoverAcentos(this string text)
        {
            return new string(text
                .Normalize(NormalizationForm.FormD)
                .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                .ToArray());
        }

        //Função para fazer tratamento de Letras Maisculas e Minúsculas
        //Ex: "PAULO PEREIRA DE ASSIS"  ou "paulo PereiRa De assiS"
        //O sistema retorna "Paulo Pereira de Assis"  
        public static String ProperCase(this string strVar)
        {
            string resultado = string.Empty;

            //Valida se a string tem conteúdo
            if (!string.IsNullOrEmpty(strVar))
            {
                //Transforma toda a string em minúsculo
                strVar = strVar.ToLower();

                //Quebra a string em palavras
                string[] array = strVar.Split(' ');

                foreach (string word in array)
                {
                    //Verifica a existência de preposições para mantê-las minúsculas
                    if (!word.Equals("e") && !word.Equals("de") && !word.Equals("da") && !word.Equals("do") && !word.Equals("das") && !word.Equals("dos"))
                    {
                        //Trata a primeira letra de cada palavra para maiúscula
                        resultado += string.Concat(word.Substring(0, 1).ToUpper(), word.Substring(1, word.Length - 1).ToLower(), " ");
                    }
                    else
                    {
                        //Concatena com as preposições
                        resultado += string.Concat(word, " ");
                    }
                }

            }

            resultado = resultado.TrimEnd();//Remove o espaço do final da string;

            return(resultado);
        }


        /// <Historico> 01/06/2018 - Criacao do Metodo </Historico>
        /// <summary>
        /// Realiza a criptografioa SHA1
        /// </summary>
        /// <param name="senha">Senha a ser criptografada</param>
        /// <returns>Senha criptografada.</returns>
        /// <example> Util.CalculateSHA1(usuario) </example>
        /// 
        public static string CalculateSHA1(string text)
        {
            try
            {
                byte[] buffer = System.Text.Encoding.Default.GetBytes(text);
                System.Security.Cryptography.SHA1CryptoServiceProvider cryptoTransformSHA1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                string hash = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
                return hash;
            }
            catch (Exception x)
            {
                throw new Exception(x.Message);
            }
        }

         
        /// <Historico> 01/08/2018 - Criacao do Metodo </Historico>
        /// <summary>
        /// Retira os espaços em branco de todos atributos string de um objeto
        /// </summary>
        /// <param name="o">Objeto a ser tratado</param>
        /// <example> Util.AplicarTrimTodosCampos(usuario) </example>
        /// 
        public static void AplicarTrimTodosCampos(Object o)
        {
            // Carregar os tipos atributos relacionados
            Type classeType = o.GetType();

            // Varre os atributos da classe
            PropertyInfo[] properties = classeType.GetProperties();
            foreach (var propertyInfo in properties)
            {
                // Pocessa somente atributos String               
                if (propertyInfo.PropertyType.Name.ToLower().Equals("string"))
                {

                    // Busca dinamicamente o atributo
                    string nomeAtributo = propertyInfo.Name;

                    string valor = "";

                    try
                    {
                        // Busca o valor do atributo
                        valor = (string)o.GetType().InvokeMember(nomeAtributo,
                        BindingFlags.GetField | BindingFlags.GetProperty, null, o, null);

                        // Aplica o trim
                        valor = valor.Trim();

                        // Seta o valor de volta no objeto
                        o.GetType().InvokeMember(nomeAtributo,
                            BindingFlags.SetProperty | BindingFlags.SetField, null, o, new Object[] { valor });
                    }
                    catch { }
                }
            }
        }

         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Retira de uma frase os caracterers numéricos
        /// </summary>
        /// <param name="dados">Frase</param>
        /// <returns>Parametro de entrada sem os caracteres numéricos.</returns>
        /// <example> Util.LimLimparCaracteresNaoNumericos("Teste25") - Return "Teste" </example>
        /// 

        public static String LimparCaracteresNaoNumericos(String dados)
        {

            String saida = String.Empty;

            foreach (char c in dados)
            {
                if (Char.IsNumber(c))
                    saida += c;

            }

            return saida;

        }

         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Valida Email
        /// </summary>
        /// <param name="_email">Email a ser analisado</param>
        /// <returns>True - Email valido; (False) Email Inválido</returns>
        public static Boolean VerificarEmail(String _email)
        {

            System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");

            return (rg.IsMatch(_email));

        }

         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Gerar um nome aleatorio para arquivo que sofrerá upload.
        /// Objetivo gerar nomes de arquivos que nao sejam sequenciais (exemplo Id de tabela) e únicos. Com isso não haverá conflitos de nomes
        /// e nem identificação de arquivos sem acesso via sistema (nesse caso quando é gerado sequencialmente usuário tem condições de identificar demais arquivos)
        /// 
        /// Formato do nome do arquivo 3 numeros aleatorios + 6 caracteres que correspondem ao id do registro + 3 numeros aleatorios
        /// com isso evita uma possivel obtencao de arquivos via http jah que serah mais dificil identificar o nome dos arquivos.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_nomeArquivo"></param>
        /// <returns>Nome gerado</returns>
        /// <example>Util.GerarNomeArquivo("253", "Arquivo.pdf") - Return "358000253658.pdf"</example>
        public static String GerarNomeArquivo(String _id, String _nomeArquivo)
        {

            String extensao = _nomeArquivo;

            int pos = 0;

            do
            {

                pos = extensao.IndexOf(".");

                if (pos >= 0)
                    extensao = extensao.Substring(pos + 1);


            } while (pos >= 0);

            // Formato do nome do arquivo 3 numeros aleatorios + 6 caracteres que correspondem ao id do registro + 3 numeros aleatorios
            //com isso evita uma possivel obtencao de arquivos via http jah que serah mais dificil identificar o nome dos arquivos.
            Random random = new Random();

            return random.Next(100, 999).ToString("000") + _id.Trim().PadLeft(6, '0') + random.Next(100, 999).ToString("000") + "." + extensao;

        }

         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        ///  Verifica se a frase estah em caixa alta
        /// </summary>
        /// <param name="_valor"></param>
        /// <returns></returns>
        public static Boolean VerificaCampoCaixaAlta(String _valor)
        {
            return (_valor.Equals(_valor.ToUpper()));
        }

         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Campo decimal que deve ser usado como String no formato brasileiro (Ex. Usar em um TextBox de uma frame)
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        /// <example>Util.CampoDecimalBancoDadosParaFrame(3500.58) - Return 3500,58 </example>
        /// <example>Util.CampoDecimalBancoDadosParaFrame(79) - Return 79,00 </example>
        public static String CampoDecimalBancoDadosParaFrame(String valor)
        {

            if (!String.IsNullOrEmpty(valor))
                valor = valor.Replace(".", ",");
            else
                valor = "0,00";

            if (valor.IndexOf(",") == -1)
                valor += ",00";

            return valor;

        }

         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Campo String contendo uma informação numérica com casas decimais no formato brasileiro. Esse será convertido para formato brasileiro (Podendo ser salvo no BD)
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        /// <example>Util.CampoDecimalFrameParaBancoDados(3500,58) - Return 3500.58 </example>
        /// <example>Util.CampoDecimalFrameParaBancoDados(79) - Return 79.00 </example>
        public static String CampoDecimalFrameParaBancoDados(String valor)
        {
            if (!String.IsNullOrEmpty(valor))
            {
                valor = valor.Replace(".", "");
                valor = valor.Replace(",", ".");
            }
            else
                valor = "0.00";

            if (valor.IndexOf(".") == -1)
                valor += ".00";

            return valor;
        }

         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Campo String recuperado do banco de dados e convertido para o formato de hora para ser exibido em frame
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static String CampoHoraMinutoBancoDadosParaFrame(String valor)
        {
            DateTime data;
            String saida = "00:00";

            if (!String.IsNullOrEmpty(valor))
            {
                try
                {
                    data = Convert.ToDateTime(valor, new System.Globalization.CultureInfo("pt-BR"));

                    if (data.Hour.ToString().Length == 1)
                        saida = "0" + data.Hour.ToString();
                    else
                        saida = data.Hour.ToString();

                    if (data.Minute.ToString().Length == 1)
                        saida += ":0" + data.Minute.ToString();
                    else
                        saida += ":" + data.Minute.ToString();

                }
                catch (Exception e)
                {
                    saida = "99:99";
                }
            }

            return saida;

        }

         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Campo String recuperado de frame e convertido para ser utilizado em banco de dados
        /// OBS: Não precisa incluir aspas
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static String CampoDataFrameParaBancoDados(String valor)
        {

            /*
             * Alexandre MP
             * Metodo deve ser usado somente apos recuperar dados de um campo decimal e queira 
             * ser utilizado em frame para ser exibido em TextBox ou similar. O simbolo deciaml 
             * será alterado para o Br
             * */
            String saida = " NULL ";

            if (!String.IsNullOrEmpty(valor))
                if (valor.Length >= 10)
                    saida = "'" + valor.Substring(6, 4) + "-" + valor.Substring(3, 2) + "-" + valor.Substring(0, 2) + "'";

            return saida;

        }

         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// String a ser utilizada em montagem de sql  
        /// OBS: Não precisa incluir aspas
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static String VerCampoVazio(String valor)
        {

            String retorno = valor;

            if (String.IsNullOrEmpty(valor))
                retorno = " NULL ";

            return retorno;

        }


         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Verifica duas datas no formato dd/mm/yyyy 
        /// Retorno -1 se data inicial for maior 
        /// Retorno 0 se datas iguais 
        /// Retorno 1 se data final for maior 
        /// 
        /// </summary>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <returns></returns>
        public static int VerificarIntervaloData(String dataInicial, String dataFinal)
        {
            int retorno = 0;

            try
            {
                DateTime dt1 = new DateTime(Convert.ToInt32(dataInicial.Substring(6, 4)), Convert.ToInt32(dataInicial.Substring(3, 2)), Convert.ToInt32(dataInicial.Substring(0, 2)));
                DateTime dt2 = new DateTime(Convert.ToInt32(dataFinal.Substring(6, 4)), Convert.ToInt32(dataFinal.Substring(3, 2)), Convert.ToInt32(dataFinal.Substring(0, 2)));

                if (dt1 > dt2)
                    retorno = -1;
                else
                    if (dt1 < dt2)
                        retorno = 1;

            }
            catch (Exception e)
            {
                throw e;
            }

            return retorno;

        }

         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Verifica duas datas (hora) no formato hh:mm
        /// Retorno -1 se data inicial for maior 
        /// Retorno 0 se datas iguais 
        /// Retorno 1 se data final for maior 
        /// </summary>
        /// <param name="horaInicial"></param>
        /// <param name="horaFinal"></param>
        /// <returns></returns>
        public static int VerificarIntervaloHora(String horaInicial, String horaFinal)
        {
            int retorno = 0;

            try
            {
                DateTime dt1 = new DateTime(2000, 01, 01, Convert.ToInt32(horaInicial.Substring(0, 2)), Convert.ToInt32(horaInicial.Substring(3, 2)), 0);
                DateTime dt2 = new DateTime(2000, 01, 01, Convert.ToInt32(horaFinal.Substring(0, 2)), Convert.ToInt32(horaFinal.Substring(3, 2)), 0);

                if (dt1 > dt2)
                    retorno = -1;
                else
                    if (dt1 < dt2)
                        retorno = 1;

            }
            catch (Exception e)
            {
                throw e;
            }

            return retorno;

        }


         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Verifica se data eh valida
        /// Formato esperado dd/mm/yyyy
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Boolean VerificarData(String data)
        {
            Boolean retorno = true;

            try
            {
                DateTime dt1 = new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2)));

            }
            catch (Exception e)
            {
                retorno = false;
            }

            return retorno;

        }

         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Verifica se hora eh valida
        /// Formato esperado hh:mm
        /// </summary>
        /// <param name="hora"></param>
        /// <returns></returns>
        public static Boolean VerificarHora(String hora)
        {
            Boolean retorno = true;

            try
            {
                DateTime dt1 = new DateTime(2000, 01, 01, Convert.ToInt32(hora.Substring(0, 2)), Convert.ToInt32(hora.Substring(3, 2)), 0);

            }
            catch (Exception e)
            {
                retorno = false;
            }

            return retorno;

        }

         
        /// <Historico> 28/01/2018 - Criacao do Metodo </Historico>
        /// <summary>
        /// Valida CNPJ
        /// </summary>
        /// <param name="CNPJ"></param>
        /// <returns></returns>
        public static bool ValidaCNPJ(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }

         
        /// <Historico> 01/05/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Valida CPF
        /// </summary>
        /// <param name="vrCPF"></param>
        /// <returns></returns>
        public static bool ValidaCPF(string vrCPF)
        {
            String valor = vrCPF.Replace(".", "");

            valor = valor.Replace("-", "");

            if (valor.Length != 11)
                return false;

            bool igual = true;

            for (int i = 1; i < 11 && igual; i++)
                if (valor[i] != valor[0])
                    igual = false;

            if (igual || valor == "12345678909")
                return false;

            int[] numeros = new int[11];

            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(valor[i].ToString());

            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];

            int resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }
            else
                if (numeros[9] != 11 - resultado)
                    return false;

            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else
                if (numeros[10] != 11 - resultado)
                    return false;

            return true;
        }


         
        /// <Historico> 15/07/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Retira espacamentos desnecessarios em uma sequencia
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static String RetirarEspacosDesnecessarios(String texto)
        {
            String saida = String.Empty;
            int ponteiro = 0;
            int tamParametro = 0;
            String letra = String.Empty;

            texto = texto.Trim();
            tamParametro = texto.Length;

            // Retira espacos desnecessarios
            while (ponteiro < tamParametro)
            {
                letra = texto.Substring(ponteiro, 1);

                // Ignora mais de um espaco
                if (ponteiro != 0)
                    if ((texto.Substring((ponteiro - 1), 1).Equals(letra)) && (letra.Equals(' ')))
                    {
                        ponteiro++;
                        continue;
                    }

                saida += letra;
                ponteiro++;
            }
            return saida;
        }

         
        /// <Historico> 15/07/2014 - Criacao do Metodo </Historico>
        /// <summary>
        /// Separa as palavras do texto em um vetor
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static String[] ExtraixPalavras(String texto)
        {
            return Util.RetirarEspacosDesnecessarios(texto).Split(' ');
        }

         
        /// <Historico> 26/03/2018 - Criacao do Metodo </Historico>
        /// <summary>
        /// Extrai data por extenso
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static String BuscarDataExtenso(DateTime dataRef)
        {

            CultureInfo culture = new CultureInfo("pt-BR");
            DateTimeFormatInfo dtfi = culture.DateTimeFormat;

            //DateTime.no
            int dia = dataRef.Day;
            int ano = dataRef.Year;
            string mes = culture.TextInfo.ToTitleCase(dtfi.GetMonthName(dataRef.Month));
            string diasemana = culture.TextInfo.ToTitleCase(dtfi.GetDayName(dataRef.DayOfWeek));
            string data = diasemana + ", " + dia + " de " + mes + " de " + ano;

            return data;

        }


        /// <summary>
        /// Inverte o formado da data: 'yyyy-MM-dd' para 'dd-MM-yyy'
        /// Caso a entrada seja diferente de 'yyyy-MM-dd' retorna o valor de origem
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static String inverteFormatoData(String data)
        {
            try
            {
                DateTime dataX = DateTime.ParseExact(data, "yyyy-MM-dd", new CultureInfo("pt-BR"));
                data = dataX.ToString("dd/MM/yyyy");
            }
            catch{}

            return data;
        }

        

        #region //Registros de auditoria - Busca por IP e Hostname

        //RETORNA O IP REAL DA INTERNET (REALIZA BUSCA EM SITE EXTERNO)
        public static String BuscarIpValido()
        {
            string strIP = string.Empty;
            
            try
            {
                WebClient wc = new WebClientWithTimeout();
                strIP = wc.DownloadString("https://api.ipify.org");
            }
            catch (Exception e)
            {
                strIP = "time-out";
            }

            return (strIP);
        }
        
        //Verifica o tempo de acesso a página externa - TimeOut
        public class WebClientWithTimeout : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest wr = base.GetWebRequest(address);
                wr.Timeout = 1000; // timeout in milliseconds (ms)
                return wr;
            }
        }

        //RETORNA O HOSTNAME (DA MÁQUINA LOCAL)
        public static String BuscarHostname()
        {
            String Hostname = string.Empty;

            try
            {
                //Hostname = System.Net.Dns.GetHostName(); //traz o Host www
                Hostname = System.Net.Dns.GetHostEntry(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_HOST"]).HostName; //host do UTM
                
            }
            catch(Exception e)
            {
                Hostname = "Falha";
            }

            return (Hostname);
        }

        //RETORNA O IP (DA MÁQUINA LOCAL)
        public static String BuscarIpLocal()
        {
            string strIP = string.Empty;

            try
            {
                strIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch (Exception e)
            {
                strIP = "Falha";
            }


            
            return (strIP);
        }

        #endregion



        /// <summary>
        /// Método que quebra uma string em palavras e agrupa em uma lista
        /// </summary>
        /// <param name="textoAQuebrar"></param>
        /// <returns></returns>
        public static List<String> criarListadePalavras(String textoAQuebrar)
        {
            //Pegar uma string e separar as palavras em uma lista,
            //Ex: Entrada = "A Raposa Cruzou A Rua";
            //    Saida   = 
            //              List[0] = "A"
            //              List[1] = "Raposa"
            //              List[2] = "Cruzou"
            //              List[3] = "A"
            //              List[4] = "Rua"

            List<String> palavrasDetectadas = new List<string>();

            int indiceUltimoCaractere = textoAQuebrar.Length - 1;

            for (int i = 0; i <= indiceUltimoCaractere; i++)
            {
                String strPalavra = String.Empty;
                while (textoAQuebrar[i] != ' ' && i < indiceUltimoCaractere)
                {
                    strPalavra += textoAQuebrar[i];
                    i++;

                    if (i == indiceUltimoCaractere)
                    {
                        strPalavra += textoAQuebrar[i];
                    }
                }
                if(strPalavra.Length > 2)
                    palavrasDetectadas.Add(strPalavra);
            }
            return palavrasDetectadas;
        }

        
        /// <summary>
        /// Método para montar string de pesquisa concatenando com as expressões NEAR para uso do SQL Server Full-Text Indexing
        /// </summary>
        /// <param name="textoAQuebrar"></param>
        /// <returns></returns>
        public static String criarParametroContains(String textoAQuebrar)
        {

            //Pegar uma lista de palavras e construir uma string usada no parametro de Contains
            //Ex: Entrada = 
            //              List[1] = "A"
            //              List[2] = "Raposa"
            //              List[3] = "Cruzou"
            //              List[4] = "A"
            //              List[5] = "Rua"
            //    Saida = "'*A*' NEAR A NEAR Raposa NEAR Cruzou NEAR A NEAR Rua "

            String strParametroContains = "NEAR(( \"";

            List<String> palavrasDetectadas = criarListadePalavras(textoAQuebrar);
            if (palavrasDetectadas.Count == 1) return "\"" + palavrasDetectadas[0] + "*\"";
            for (int i = 0; i < palavrasDetectadas.Count; i++)
            {
                if (i == 0)
                {
                    strParametroContains += palavrasDetectadas[i] + "*\"";
                }
                else
                {
                    strParametroContains += ", \"" + palavrasDetectadas[i] + "*\"";
                }

            }
            strParametroContains += "), 3)";

            return strParametroContains;
        }



    }
}