from typing import List


class Label:
    start: int
    end: int
    label: str
    value: str

    def __init__(self, start: int, end: int, label: str, value:str): # rm -> None
        self.start = start
        self.end = end
        self.label = label
        self.value = value

    def to_dict(self) -> dict:
        return {
            "start": self.start,
            "end": self.end,
            "label": self.label,
            "value":self.value
        }



class Document:
    content: str
    labels: List[dict]
    id:int

    def __init__(self,id:int, content: str, labels: List[dict]): # rm -> None
        self.id= id
        self.content = content
        self.labels = labels

    def to_dict(self) -> dict:
        return {
            "id":self.id,
            "content": self.content,
            "labels": self.labels,
        }

