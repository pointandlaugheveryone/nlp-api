from openai import AzureOpenAI
import azure.keyvault.secrets as azk
from azure.identity import DefaultAzureCredential
import os, asyncio
from pydantic import json
from NERtemplate import NamedEntities


def get_key(keyname):
    vault_uri = 'https://keyvault-labeling.vault.azure.net/'
    client = azk.SecretClient(vault_uri, DefaultAzureCredential())
    secret = client.get_secret(keyname)
    key = secret.value
    return key


async def make_request(data:str):
    key = get_key('openai-key') 
    endpoint = os.getenv("AZURE_OPENAI_ENDPOINT") or ''
    
    client = AzureOpenAI(
    azure_endpoint = endpoint, 
    api_key=key,  
    api_version="2024-12-01-preview"
    )

    completion = client.beta.chat.completions.parse(
        model="gpt-4.1-nano",
        messages=[
            {
                "role": "user",
                "content": """You are a Named Entity Recognition (NER) assistant.
                Your job is to identify and return all entities and their text contents as they are (including phrases), for a given piece of text. 
                The input will always be in the czech language.
                You are to strictly conform only to the following entity types: 
                softSkill, domainSpecificSkill, personalityTrait and jobTitle. 
                If uncertain about entity type or start/end index, ignore it or leave blank.
                Score (0 to 1) should express how much you think the label fits the entityName's meaning.
                In the respective text fields, fill in the original text without any changes.""",
            },
            {
                "role": "user",
                "content": data,
            }
        ],
        response_format=NamedEntities,
        n=1,
        temperature=0,
        
    )
    response_str = str(completion.choices[0].message.parsed)
    print(response_str)

    response = NamedEntities.parse_raw(response_str)


async def main():
    file_UUID = '0a5653a1-9f75-4c01-a3d6-f13e58d1d928' # test requests (output1.txt)
    with open(f'{os.getcwd()}/data_cs/{file_UUID}.txt','r') as f:
        text = f.read()
    await make_request(text)

asyncio.run(main())
