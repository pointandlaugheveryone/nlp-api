from openai import AzureOpenAI
import azure.keyvault.secrets as azk
from azure.identity import DefaultAzureCredential
from azure.core.credentials import AzureKeyCredential
import os


def get_key(keyname):
    vault_name = 'keyvault-labeling'
    vault_uri = 'https://keyvault-labeling.vault.azure.net/'
    client = azk.SecretClient(vault_uri, DefaultAzureCredential())
    secret = client.get_secret(keyname)
    key = secret.value
    return key


def make_request():
    key = get_key('openai-key') or ''
    client = AzureOpenAI(
        api_version="2024-12-01-preview",
        endpoint="https://veron-mcj2dp5i-eastus2.cognitiveservices.azure.com/",
        credential=AzureKeyCredential(key)
    )
    completion = client.chat.completions.create(
        messages=[
            {
                "role": "user",
                "content": "instructions",
            },
            {
                "role": "user",
                "content": "json",
            }
        ],
        n=1,
        temperature=0,
        model="gpt-4.1-nano"
    )
    response = completion.choices[0].message.content
    return response