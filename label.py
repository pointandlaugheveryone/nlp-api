from openai import AzureOpenAI
import azure.keyvault.secrets as azk
from azure.identity import DefaultAzureCredential
import os,json, asyncio
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
                Your job is to identify and return all entity names and their types for a given piece of text. 
                The input will always be in the czech language.
                You are to strictly conform only to the following entity types: 
                softSkill, domainSpecificSkill, personalityTrait and jobTitle. 
                If uncertain about entity type or start/end index, ignore it or leave blank.
                In the respective text fields, fill in the original text without any changes.""",
            },
            {
                "role": "user",
                "content": data,
            }
        ],
        response_format=NamedEntities,# TODO
        n=1,
        temperature=0,
        
    )
    response = completion.choices[0].message.parsed
    print(response)


async def main():
    with open('/home/ronji/repos/nlp-api/data_cs/0a5653a1-9f75-4c01-a3d6-f13e58d1d928.txt','r') as f:
        text = f.read()
    await make_request(text)

asyncio.run(main())
