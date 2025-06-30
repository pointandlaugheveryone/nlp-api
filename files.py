# used to move data from a different location

import os, csv, io


def txt_to_csv():
    path ='/home/ronji/repos/nlp-api/data_cs/'
    with open('/home/ronji/repos/nlp-api/resumes_cs.csv', 'w', encoding='UTF8') as csv_file:
        writer = csv.writer(csv_file)
        writer.writerow(["id", "contents"])
        for f in os.listdir(path):
            with open(os.path.join(path,f),'r') as file:
                line = [file.name,file.read()]
                writer.writerow(line)


txt_to_csv()
    
    
    

    