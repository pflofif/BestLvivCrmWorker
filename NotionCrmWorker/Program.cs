using Notion.Client;

string notionIntegrationToken = "secret_WTI48bIta9AW6BCVzdw2H951XtDvwQdVubYLPkiiw7X";
string companiesDatabaseId = "58bf0c7b5e8c4ab69b1cee528b04971b";
string informationBookDatabaseId = "4a6f9258bbfb44b7b577f691ea48c404";
var client = NotionClientFactory.Create(new ClientOptions
{
    AuthToken = notionIntegrationToken
});

var companies = await GetDatabaseEntries(companiesDatabaseId);
var informationBookEntries = await GetDatabaseEntries(informationBookDatabaseId);

List<Task> updateOperations = new();
foreach (var personInformation in informationBookEntries)
{
    var personName = ((TitlePropertyValue) personInformation.Properties["Повне ім'я"]).Title.First().PlainText;

    var companyEntries =
        companies.Where(entry =>
            {
                try
                {
                    return ((RichTextPropertyValue) entry.Properties["Відповідальний(-на)"])
                        .RichText.First().PlainText == personName;
                }
                catch (Exception e)
                {
                    return false;
                }
            })
            .ToList();

    foreach (var company in companyEntries)
    {
        updateOperations.Add(UpdateCompanyRelation(company.Id, personInformation.Id));
    }
    
    Console.WriteLine($"Завершено створення посилань для: {personName}");
}

await Task.WhenAll(updateOperations);
Console.WriteLine("End of program");

async Task UpdateCompanyRelation(string companyId, string informationBookEntryId)
{
    var propertiesToUpdate = new Dictionary<string, PropertyValue>
    {
        {
            "посилання",
            new RelationPropertyValue
            {
                Relation = new List<ObjectId> {new() {Id = informationBookEntryId}}
            }
        }
    };

    await client.Pages.UpdatePropertiesAsync(companyId, propertiesToUpdate);
}


async Task<List<Page>> GetDatabaseEntries(string databaseId)
{
    List<Page> pages = new List<Page>();
    string? startCursor = null;
    do
    {
        var queryParameters = new DatabasesQueryParameters
        {
            StartCursor = startCursor
        };
        var response = await client.Databases.QueryAsync(databaseId, queryParameters);
        pages.AddRange(response.Results);
        startCursor = response.HasMore ? response.NextCursor : null;
    } while (startCursor != null);

    return pages;
}