using System.Net;
using System.Net.Sockets;
using System.Text;

public class ServidorHttp
{
    private TcpListener Controlador { get; set; }
    //Fica escutando uma porta de rede do PC a espera de qualquer tipo de comunicaçãoTCP
   

    private int Porta { get; set; }//qual porta será escutada
    private int QtdeRequests { get; set; }//contador para ver se alguma requisicao esta sendo perdida 
    public string HtmlExemplo { get;  set; }

    public ServidorHttp(int porta = 8080)
    {

        this.Porta = porta;
        this.CriarHtmlExemplo();
        try
        {
            this.Controlador = new TcpListener(IPAddress.Parse("127.0.0.1"), this.Porta);//escuta a porta no ip local
            this.Controlador.Start();
            Console.WriteLine($"Servidor rodando na porta {this.Porta} no endpoint http://localhost:{this.Porta}");
            Task servidorHttpTask = Task.Run(() => AguardarRequests());
            servidorHttpTask.GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            //Caso ocorra algum erro, como a tentativa de utilizar uma porta ja em uso
            Console.WriteLine($"Erro ao iniciar o servidor na porta {this.Porta}:\n {e.Message}");
        }

    }
    private async Task AguardarRequests()
    {
        while (true)
        {
            Socket conexao = await this.Controlador.AcceptSocketAsync();//eespera a requisicao
            this.QtdeRequests++;//quando recebe a requisicao, o await cai pra linha de baixo, onde qntd request é incrementado         }
            Task task = Task.Run(() => ProcessarRequest(conexao, this.QtdeRequests));

        }
    }
    private void ProcessarRequest(Socket conexao, int numeroRequest)
    {
        Console.WriteLine($"Processando request #{numeroRequest}...\n");
        if (conexao.Connected)
        {//verifica se a conecao esta ativa
            byte[] bytesRequisicao = new byte[1024];//espaco que armazena a request
            conexao.Receive(bytesRequisicao, bytesRequisicao.Length, 0);
            //preenche o vetor de bytes com os dados recebidos do navegador do usuario
            //conecao.receive(ONDE GUARDAR, QUANTOS BYTES SERAO LIDOS, A PARTIR DE QUAL POSICAO)
            string textoRequisicao = Encoding.UTF8.GetString(bytesRequisicao).Replace((char)0, ' ').Trim();
            //Descodifica a requisicao, transformando no formato texto UTF8.
            //replace vai substituir o caracter 0 por espacos, e o trim eliminara os espacos
           //isso é necessario pois o vetor é iniciado totalmente prenchidos por zeros e, todos os espacos nao preenchidos, continuarao com zeros. Assim,pode-se elimina-los para economizar espacos
            if (textoRequisicao.Length > 0)
            {
                Console.WriteLine($"\n{textoRequisicao}\n");
                Console.WriteLine($"{textoRequisicao}\n");
                string[] linhas=textoRequisicao.Split("\r\n");
                int iPrimeiroEspaco = linhas[0].IndexOf(' ');
int iSegundoEspaco = linhas[0].LastIndexOf(' ');
string metodoHttp = linhas[0].Substring(0, iPrimeiroEspaco);
string recursoBuscado = linhas[0].Substring(
iPrimeiroEspaco + 1, iSegundoEspaco - iPrimeiroEspaco - 1);
string versaoHttp = linhas[0].Substring(iSegundoEspaco + 1);
iPrimeiroEspaco = linhas[1].IndexOf(' ');
string nomeHost = linhas[1].Substring(iPrimeiroEspaco + 1);

/*                 var bytesConteudo=LerArquivo(recursoBuscado);
 *//*                 var bytesConteudo = Encoding.UTF8.GetBytes(this.HtmlExemplo, 0, this.HtmlExemplo.Length);
 */
/*                 var bytesCabecalho = GerarCabecalho(versaoHttp, "text/html;charset=utf-8", "200", bytesConteudo.Length);
                    
    */byte[] bytesCabecalho=null;
    var bytesConteudo=LerArquivo(recursoBuscado);         
    if(bytesConteudo.Length>0){

        bytesCabecalho=GerarCabecalho(versaoHttp,"text/html;charset=utf-8","200",bytesConteudo.Length);
    }      
    else{
        bytesConteudo = Encoding.UTF8.GetBytes(
    "<h1>Erro 404 - Arquivo Não Encontrado</h1>");
bytesCabecalho = GerarCabecalho(versaoHttp, "text/html; charset=utf-8",
    "404", bytesConteudo.Length);

    }
  int bytesEnviados = conexao.Send(bytesCabecalho, bytesCabecalho.Length, 0);
                bytesEnviados+=conexao.Send(bytesConteudo,bytesConteudo.Length,0);
                conexao.Close();
                Console.WriteLine($"{bytesEnviados} bytes enviados em resposta à requisição #{numeroRequest}.");

            }
        }
        Console.WriteLine($"\n Request {numeroRequest} finalizado");


    }
    public byte[] GerarCabecalho(string versaoHttp, string tipoMime,
    string codigoHttp, int qtdeBytes = 0)
    {
        StringBuilder texto = new StringBuilder();
        texto.Append($"{versaoHttp} {codigoHttp}{Environment.NewLine}");
        texto.Append($"Server: Servidor Http Simples 1.0{Environment.NewLine}");
        texto.Append($"Content-Type: {tipoMime}{Environment.NewLine}");
        texto.Append($"Content-Length: {qtdeBytes}{Environment.NewLine}{Environment.NewLine}");
        return Encoding.UTF8.GetBytes(texto.ToString());
    }
    private void CriarHtmlExemplo()
{
    StringBuilder html = new StringBuilder();
    html.Append("<!DOCTYPE html><html lang=\"pt-br\"><head><meta charset=\"UTF-8\">");
    html.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
    html.Append("<title>Página Estática</title></head><body>");
    html.Append("<h1>Página Estática</h1></body></html>");
    this.HtmlExemplo = html.ToString();
}
public byte[] LerArquivo(string recurso){
/*      string diretorio= "C:\\Users\\saulo\\OneDrive\\Área de Trabalho\\DEV\\Asp.net\\servidorHTTP\\www";//caminho do meu computador, para testar

 */   
 string diretorio = Path.Combine(Directory.GetCurrentDirectory(), "www");

   string caminhoArquivo= diretorio+recurso.Replace("/","\\");
   if (File.Exists(caminhoArquivo))
{
    return File.ReadAllBytes(caminhoArquivo);
}
else return new byte[0];

}

}
