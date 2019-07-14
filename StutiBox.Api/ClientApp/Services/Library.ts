import {ILibraryState} from '../Models/Library'
import {HubConnectionBuilder, HubConnection} from '@aspnet/signalr'
class LibraryService {
    static LibraryHubConnection: HubConnection
    static onLibraryStateChange: any
    static buildHub=async ()=>{
        const hubBuilder = new HubConnectionBuilder()
        LibraryService.LibraryHubConnection = hubBuilder
            .withUrl('/librarystatus')
            .withAutomaticReconnect()
            .build()
        LibraryService.LibraryHubConnection.on('ReceiveLibraryStatus',LibraryService.libraryStatusReceived)
        await LibraryService.LibraryHubConnection.start()
        await LibraryService.GetLibraryItems()
    }
    static initialize = async(stateChangeCallback:any)=>{
        LibraryService.onLibraryStateChange=stateChangeCallback
        await LibraryService.buildHub();
    }
    static libraryStatusReceived=(state:ILibraryState)=>{
        if(LibraryService.onLibraryStateChange)
            LibraryService.onLibraryStateChange(state);
    }
    static GetLibraryItems=async()=> {
        const state = await LibraryService.LibraryHubConnection.invoke<ILibraryState>('getLibraryItems')
        LibraryService.libraryStatusReceived(state)
    }
    static refreshLibrary=async() => {
        await LibraryService.LibraryHubConnection.invoke<ILibraryState>('refresh',false)
    }
}
export default LibraryService