import { Card } from "./cardModel"
import { Image } from "./imageModel"

export class Story{
    IDSTORY:number
    NAME:string
    IMAGE:Image
    TAGS:any[]
    CARDS:Card[]
    DESCRIPTION:string
    FAVOURITE:number
    SHARED:number
    PROFILEID:number
    NumberOfLikes:number
}