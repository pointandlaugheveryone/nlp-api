from openai import AzureOpenAI
import rich
import azure.keyvault.secrets as azk
from azure.identity import DefaultAzureCredential
import os, asyncio, json
from data_models import Labels


def get_key(keyname):
    vault_uri = 'https://keyvault-labeling.vault.azure.net/'
    client = azk.SecretClient(vault_uri, DefaultAzureCredential())
    secret = client.get_secret(keyname)
    key = secret.value
    return key


async def send_request(data:str):
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
                "role": "system",
                "content": """You are a Named Entity Recognition (NER) model used as an advanced ATS scanner.
                Your job is to extract words matching the specified entity types; soft skill, Capability, Personality trait, Job title.
                Consider 'Capability' as a field-specific skill, for example experience with specific software or methodology.
                Do not reformat the text you find in the original document in any way.
                """,
            },
            {
                "role": "user",
                "content": data,
            }
        ],
        response_format=Labels,
        n=1,
        temperature=0
        
    )
    message = completion.choices[0].message
    if (message.refusal):
        rich.print(message.refusal)
    else:
        labels_dict = Labels.model_validate(message.parsed).model_dump()
        


async def format_labels(filename:str):
    with open(f'{os.getcwd()}/data_src/{filename},txt','r') as f:
        text = f.read()
    await send_request(text)
    # TODO




asyncio.run(send_request("77.txt"))
