require('dotenv').config();
const { Client } = require('@notionhq/client');

const notion = new Client({
    auth: process.env.NOTION_API_KEY,
});

const companiesDatabaseId = process.env.COMPANIES_DB_ID;
const informationBookDatabaseId = process.env.INFOBOOK_DB_ID;
const projectsId = process.env.PROJECTS_ID;

async function main() {
    try {
        const companies = await getDatabaseEntries(companiesDatabaseId);
        const informationBookEntries = await getDatabaseEntries(informationBookDatabaseId);
        const projects = await getDatabaseEntries(projectsId);

        //  createRelationsBetweenCompaniesAndPeople(informationBookEntries, companies);
        //console.log("Всі звязки успішно створені.");

        //console.log(projects)

        createRelationsBetweenCompaniesAndProjects(companies, projects);
        console.log("Всі компанії поєднанні з проєктами.");

        //addCommentsToPage(companies);
        //console.log("Всі коментарі успішно додані.");
    } catch (error) {
        console.error("Помилка при виконанні скрипта:", error);
    }
}

async function addCommentsToPage(companies) {
    const commentTasks = companies.map(async company => {
        const commentText = getCommentFromProperties(company.properties);

        if (commentText) {
            await addCommentToPage(company.id, commentText);
            console.log(`Коментар "${commentText}" додано до сторінки з ID: ${company.id}`);
        } else {
            console.log(`Сторінка з ID: ${company.id} не має коментаря у властивості "Коментар".`);
        }
    });

    await Promise.all(commentTasks);
}

async function createRelationsBetweenCompaniesAndProjects(companies, projects) {
    // whan company contain 1 relation to project
    const updateTasks = projects.slice(0, 2).flatMap(project => {
        const projectName = project.properties["Назва проекту"].title[0].plain_text;
        console.log(projectName, "- Звязок в процесі")
        const i = companies.filter(company => {
            try {
                const project = company.properties["Проєкт"].rich_text[0].plain_text;
                return project == projectName;
            } catch {
                return false;
            }
        }).map(company => updateCompanyWithProjectRelation(company.id, project.id));
    })

    await Promise.all(updateTasks);

    // create multiple
    // const updateTasks = projects.flatMap(project => {
    //     const projectName = project.properties["Назва проекту"].title[0].plain_text;
    //     console.log(projectName, "- Звязок в процесі");

    //     const i = companies.filter(company => {
    //         try {
    //             const project = company.properties["Проєкт"].rich_text[0].plain_text;

    //             return project.includes(", ");
    //             //return project == projectName;
    //         } catch {
    //             return false;
    //         }
    //     }).map(company => {
    //         updateCompaniesSeveralRelationships(company.id, projects[0].id, projects[1].id);
    //     })
    //     //.map(company => updateCompanyWithProjectRelation(company.id, project.id));
    // });

    // await Promise.all(updateTasks);
}


async function updateCompaniesSeveralRelationships(companyId, ejf, bec) {
    await notion.pages.update({
        page_id: companyId,
        properties: {
            "посилання проєкт": {
                type: "relation",
                relation: [{ id: ejf }, { id: bec }],
            },
        },
    });
}


async function updateCompanyWithProjectRelation(companyId, projectEntryId) {
    await notion.pages.update({
        page_id: companyId,
        properties: {
            "посилання проєкт": {
                type: "relation",
                relation: [{ id: projectEntryId }],
            },
        },
    });
}

async function createRelationsBetweenCompaniesAndPeople(informationBookEntries, companies) {
    const updateTasks = informationBookEntries.flatMap(person => {
        const personName = person.properties["Повне ім'я"].title[0].plain_text;
        console.log(personName, "- Звязок в процесі");

        return companies.filter(company => {
            try {
                const responsiblePerson = company.properties["Відповідальний(-на)"].rich_text[0].plain_text;
                return responsiblePerson == personName;
            } catch {
                return false;
            }
        }).map(company => updateCompanyRelation(company.id, person.id));
    });

    await Promise.all(updateTasks);
}


async function updateCompanyRelation(companyId, informationBookEntryId) {
    await notion.pages.update({
        page_id: companyId,
        properties: {
            "посилання": {
                type: "relation",
                relation: [{ id: informationBookEntryId }],
            },
        },
    });
}

function getCommentFromProperties(properties) {
    const commentProperty = properties["Коментар"];
    if (commentProperty && commentProperty.rich_text && commentProperty.rich_text.length > 0) {
        return commentProperty.rich_text[0].text.content;
    }
    return null;
}

async function getDatabaseEntries(databaseId) {
    const pages = [];
    let hasMore = true;
    let startCursor = undefined;

    while (hasMore) {
        const response = await notion.databases.query({
            database_id: databaseId,
            start_cursor: startCursor,
        });

        pages.push(...response.results);
        hasMore = response.has_more;
        startCursor = response.next_cursor;
    }
    return pages;
}

async function addCommentToPage(pageId, commentContent) {
    await notion.comments.create({
        parent: { page_id: pageId },
        rich_text: [{
            text: { content: commentContent },
        }],
    });
}

main().catch(console.error);
