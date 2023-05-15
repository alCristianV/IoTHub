import { User } from "./user";
import { Field } from "./field";
import { Action } from "./action";

export interface Device {
    id?: string;
    user?: User;
    userId?: string;
    connectionString?: string;
    name: string;
    online?: boolean;
    fields?: Array<Field>
    actions?: Array<Action>
    collaborators?: Array<User>
}
