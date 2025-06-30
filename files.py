# used to move data from a different location

import os, csv, io


def txt_to_csv():
    path = os.getcwd()
    with open(path.join([path,'/resumes_cs.csv']), 'w', encoding='UTF8') as csv_file:
        writer = csv.writer(csv_file)
        writer.writerow(["id", "contents"])
        for f in os.listdir(path):
            with open(os.path.join(path,f),'r') as file:
                line = [file.name,file.read()]
                writer.writerow(line)
