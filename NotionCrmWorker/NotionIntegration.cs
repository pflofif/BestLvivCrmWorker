using Notion.Client;

namespace NotionCrmWorker;

public class NotionIntegration
{
    private const string NotionIntegrationToken = "secret_WTI48bIta9AW6BCVzdw2H951XtDvwQdVubYLPkiiw7X";
    private const string CompaniesDatabaseId = "58bf0c7b5e8c4ab69b1cee528b04971b";
    private const string InformationBookDatabaseId = "4a6f9258bbfb44b7b577f691ea48c404";

    private readonly NotionClient _client = NotionClientFactory.Create(new ClientOptions
    {
        AuthToken = NotionIntegrationToken
    });

    public async Task Run()
    {
        var companies = await GetDatabaseEntries(CompaniesDatabaseId);
        var informationBookEntries = await GetDatabaseEntries(InformationBookDatabaseId);

        await UpdateCompanyResponsiblePeople(informationBookEntries, companies);
        Console.WriteLine("Всі люди успішно імпортовані до компаній");
        
        Console.WriteLine("End of program");
    }

    /// <summary>
    /// Updates the responsible people in companies from the information book entries.
    /// </summary>
    private Task UpdateCompanyResponsiblePeople(List<Page> informationBookEntries, List<Page> companies)
    {
        var updateOperations = new List<Task>();

        foreach (var personInformation in informationBookEntries)
        {
            var personName = ((TitlePropertyValue) personInformation.Properties["Повне ім'я"]).Title.First().PlainText;
            var companyEntries = companies.Where(entry => IsResponsiblePerson(entry, personName)).ToList();

            updateOperations
                .AddRange(companyEntries
                    .Select(company => UpdateCompanyRelation(company.Id, personInformation.Id)));

            Console.WriteLine($"Завершено створення посилань для: {personName}");
        }

        return Task.WhenAll(updateOperations);
    }

    /// <summary>
    /// Checks if the entry is the responsible person.
    /// </summary>
    private bool IsResponsiblePerson(Page entry, string personName)
    {
        try
        {
            return ((RichTextPropertyValue) entry.Properties["Відповідальний(-на)"])
                .RichText.First().PlainText == personName;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Updates the company relation for the given company and information book entry IDs.
    /// </summary>
    private async Task UpdateCompanyRelation(string companyId, string informationBookEntryId)
    {
        var propertiesToUpdate = new Dictionary<string, PropertyValue>
        {
            {
                "посилання",
                new RelationPropertyValue {Relation = new List<ObjectId> {new() {Id = informationBookEntryId}}}
            }
        };

        await _client.Pages.UpdatePropertiesAsync(companyId, propertiesToUpdate);
    }

    /// <summary>
    /// Retrieves entries from the specified database.
    /// </summary>
    private async Task<List<Page>> GetDatabaseEntries(string databaseId)
    {
        var pages = new List<Page>();
        string? startCursor = null;

        do
        {
            var queryParameters = new DatabasesQueryParameters {StartCursor = startCursor};
            var response = await _client.Databases.QueryAsync(databaseId, queryParameters);
            pages.AddRange(response.Results);
            startCursor = response.HasMore ? response.NextCursor : null;
        } while (startCursor != null);

        return pages;
    }
}