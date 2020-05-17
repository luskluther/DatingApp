import { Photo } from "./photo";

export interface User {
    id: number;
    username: string;
    knownAs: string;
    age: number;
    gebder: string;
    created: Date;
    lastActive: Date;
    photoUrl: string;
    city: string;
    country: string;
    interests?: string; // ? is optional properties will come after requeired properties,
    // also this interface has properties of two dtos together
    introduction?: string;
    lookingFor: string;
    photos?: Photo[];
}
