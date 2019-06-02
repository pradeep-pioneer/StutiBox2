import {ILibraryState} from '../Models/Library'
import * as Axios from 'axios'
class LibraryService {
    static async getLibraryItems(): Promise<ILibraryState> {
        return Axios.default.get<{}, Axios.AxiosResponse>(
            "/api/Library", 
            {
                "headers": {"accept": "*/*",
                "Content-Type": "application/json-patch+json"
            }}).then(response=>{
                if(response.status!=200)
                    throw new Error(response.statusText)
                if(!response.data.status)
                    throw new Error(response.data.message)
                return response.data as ILibraryState
            })
    }
    static async refreshLibrary(): Promise<ILibraryState> {
        return Axios.default.get<{}, Axios.AxiosResponse>(
            "/api/Library/Refresh?stopPlayer=false", 
            {
                "headers": {"accept": "*/*",
                "Content-Type": "application/json-patch+json"
            }}).then(response=>{
                if(response.status!=200)
                    throw new Error(response.statusText)
                if(!response.data.status)
                    throw new Error(response.data.message)
                return this.getLibraryItems()
            })
    }
}
export default LibraryService