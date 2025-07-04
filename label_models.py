from typing import List


class Annotation:
    id: int
    start: int
    end: int
    label: str
    value: str

    def __init__(self,id:int, start: int, end: int, label: str) -> None:
        self.start = start
        self.id = id
        self.end = end
        self.label = label

    def to_dict(self) -> dict:
        return {
            "id": self.id,
            "start": self.start,
            "end": self.end,
            "label": self.label
        }



class Document:
    id: int
    content: str
    annotations: List[Annotation]

    def __init__(self,id:int, content: str, annotations: List[Annotation]) -> None:
        self.id= id
        self.content = content
        self.annotations = annotations

    def to_dict(self, annotations:list) -> dict:
        return {
            "id":self.id,
            "content": self.content,
            "annotations": annotations,
        }

