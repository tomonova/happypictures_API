import { FormatModel } from "./formatModel"

export class Card{
    IDCARD:string
    TEXT:string
    PROFILEID:number
    NAME:string
    TAGS:any[]
    CardOrder: number
    SHARED: number
    FORMAT:FormatModel
}