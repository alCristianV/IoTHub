export interface Field {
    id?: String
    name: String
    code?: string
    type: String
    statistics: boolean
    error?: string
}

enum Type {
    Numeric,
    String,
    Boolean
  }
