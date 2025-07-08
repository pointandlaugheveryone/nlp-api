from openai import AzureOpenAI
import azure.keyvault.secrets as azk
from azure.identity import DefaultAzureCredential
import os, asyncio, json
from data_models import Labels
from label_models import Label, Document


def get_key(keyname):
    vault_uri = 'https://keyvault-labeling.vault.azure.net/'
    client = azk.SecretClient(vault_uri, DefaultAzureCredential())
    secret = client.get_secret(keyname)
    key = secret.value
    return key


async def send_request(filepath:str):
    with open(filepath,'r') as f:
        contents = f.read().replace("\n", " ").replace("\t", " ").replace("\r", " ")

    key = get_key('openai-key')
    endpoint = 'https://labeling-llm-0.openai.azure.com/openai/deployments/gpt-4.1-nano/chat/completions?api-version=2025-01-01-preview'
    client = AzureOpenAI(
    azure_endpoint = endpoint,
    api_key=key,
    api_version="2024-12-01-preview"
    )
    completion = client.beta.chat.completions.parse(
        model="gpt-4.1-nano",
        messages=[
            {
                "role": "system",
                "content": """You are a Named Entity Recognition (NER) model used as an advanced ATS scanner.
                Your job is to extract words matching the specified entity types; soft skill, Capability, Personality trait, Job title. 
                Skip words not matching any of these types with well enough accuracy that a human would categorise them the same.
                Only assign labels if youre certain that they are relevant in the job market and fall under one of specified categories with well enough accuracy.
                Do not reformat the text you find in the original document in any way.
                """,
            },
            {
                "role": "user",
                "content": contents,
            }
        ],
        response_format=Labels,
        n=1,

    )
    
    message = completion.choices[0].message
    if (message.refusal):
        print(f'error in request at file {filepath}:\n{message.refusal}\n\n{message}')
        return 0
    else:
        labels = Labels.model_validate(message.parsed).model_dump()['Entities']
        return (contents, labels)


def format_entities(doc_contents:str,labels_list:list,doc_id:int=0):
    doc = doc_contents
    entities = labels_list
    labeled_ents = []
    for entity in entities:
        content = entity['text']
        label = entity['type']
        if content not in doc_contents:
            continue
        else:
            start = doc_contents.find(content)
            end = start + len(content)
            labeled_ent = Label(start,end,label,content).to_dict()
            labeled_ents.append(labeled_ent)
    labeled_doc = Document(doc_id,doc_contents, labeled_ents).to_dict()
    return labeled_doc
    

async def main():
    labeled_docs = []
    for i in range(1,2456):
        label_obj = await send_request(f'{os.getcwd()}/data_src/{i}.txt')
        contents = label_obj[0] # type: ignore
        labels: list = label_obj[1]  # type: ignore
        formatted_labels = format_entities(contents,labels,i)
        labeled_docs.append(formatted_labels)
    with open(f'{os.getcwd()}/labels_en.json', 'w') as jsonfile:
        jsonfile.write(json.dumps(labeled_docs))
        

    for i in range(1,572):
        label_obj = await send_request(f'{os.getcwd()}/data_cs/{i}.txt')
        contents = label_obj[0] # type: ignore
        labels: list = label_obj[1]  # type: ignore
        formatted_labels = format_entities(contents,labels,i)
        labeled_docs.append(formatted_labels)
        if i % 10 == 0:
            with open(f'{os.getcwd()}/labels_to_{i}.json', 'w') as jsonfile:
                jsonfile.write(json.dumps(labeled_docs))
    with open(f'{os.getcwd()}/data_cs/labels_czech.json', 'w') as jsonfile:
        jsonfile.write(json.dumps(labeled_docs))


asyncio.run(main())