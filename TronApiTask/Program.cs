using Newtonsoft.Json.Linq;

public class Program
{
    public static readonly HttpClient Client = new HttpClient();
    private static async Task GetTransactionRisk(string transactionHash)
    {
        string url = $"https://apilist.tronscan.org/api/transaction-info?hash={transactionHash}";

        try
        {
            HttpResponseMessage response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            
            // Если хэш транзакции неверный, то приходит пустой JSON со статусом 200
            if (responseBody.Length <= 3)
                throw new ArgumentException("Invalid transaction hash!");
            
            JObject jsonResponse = JObject.Parse(responseBody);
            bool riskTransaction = jsonResponse.Value<bool>("riskTransaction");
            Console.WriteLine($"Risk Transaction: {riskTransaction}");

            if (jsonResponse["normalAddressInfo"] is JObject normalAddressInfo)
            {
                foreach (var address in normalAddressInfo.Properties())
                {
                    bool risk = address.Value.Value<bool>("risk");
                    Console.WriteLine($"| Address | {address.Name} | Risk | {risk}");
                }
            }
            else
            {
                Console.WriteLine("No normal address info found.");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Message :{ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occured! {ex.Message}");
        }
    }

    public static async Task Main(string[] args)
    {
        // Хэш проверочной транзакции:
        // 853793d552635f533aa982b92b35b00e63a1c1add062c099da2450a15119bcb2
        
        Console.WriteLine("Enter transaction hash:");
        string? transactionHash = Console.ReadLine();
        if (transactionHash is null)
            Console.WriteLine("Enter valid transaction hash");
        else
            await GetTransactionRisk(transactionHash);
    }
}