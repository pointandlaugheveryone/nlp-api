import os


path = '/home/ronji/repos/nlp-api/data_src/'
contents = []
dir = os.listdir('/home/ronji/repos/nlp-api/data_src/')
for f in dir:
    with open(os.path.join(path,f),'r') as file:
        contents.append(file.read())
with open('/home/ronji/repos/nlp-api/data.txt','a') as newfile:
    newfile.writelines(contents)
        