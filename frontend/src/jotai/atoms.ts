import { atom } from "jotai";
import { atomWithStorage } from "jotai/utils";

export const testAtom = atom(true); 
                              //  {Value}            или useAtomValue(testAtom)
                              //  {setValue}         или useSetValue(testAtom)
// в компоненте пишешь const [testValue, setTestValue] = useAtom(testAtom) // это простое состояние (обнуляется при перезагрузке)
export const userAtom = atomWithStorage("userFields", 
    {
        id: "",
        firstName: "Mr.",
        secoundName: "Guest",
        patronymicName: "",
        age: 0,
        grade: "",
        country: "",
        city: "",
        address: "",
        email: "add email",
        role: "guest",
    });