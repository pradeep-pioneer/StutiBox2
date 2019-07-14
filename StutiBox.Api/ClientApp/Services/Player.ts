import {IPlayerStatus, PlaybackState, RequestType, ControlRequest} from '../Models/Player'
import {HubConnectionBuilder, HubConnection} from '@aspnet/signalr'

class PlayerService {
    static PlayerHubConnection: HubConnection
    static onPlayerStateChange: any
    static buildHub=async ()=>{
        const hubBuilder = new HubConnectionBuilder()
        PlayerService.PlayerHubConnection = hubBuilder
            .withUrl('/playerstatus')
            .withAutomaticReconnect()
            .build()
        PlayerService.PlayerHubConnection.on('ReceivePlaybackStatus',PlayerService.playerStatusReceived)
        await PlayerService.PlayerHubConnection.start()
        await PlayerService.GetPlayerStatus()
    }

    private static playerStatusReceived=(playerStatus: IPlayerStatus)=>{
        if(PlayerService.onPlayerStateChange)
            PlayerService.onPlayerStateChange(playerStatus)
    }

    private static makePlayerRequest=async(requestType: RequestType, identifier: number)=> {
        const state = await PlayerService.PlayerHubConnection.invoke<IPlayerStatus>('requestAction',{requestType, identifier});
        //PlayerService.playerStatusReceived(state)
    }
    private static makePlayerControlRequest=async(controlRequest: ControlRequest, requestData: any) =>{
        const state = await PlayerService.PlayerHubConnection.invoke<IPlayerStatus>('controlAction', {controlRequest, requestData})
        //PlayerService.playerStatusReceived(state)
    }
    static initialize = async(stateChangeCallback)=>{
        PlayerService.onPlayerStateChange = stateChangeCallback
        await PlayerService.buildHub()
        //setInterval(await PlayerService.GetPlayerStatus,500)
    }
    static GetPlayerStatus = async()=> {
        const state = await PlayerService.PlayerHubConnection.invoke<IPlayerStatus>('requestPlaybackStatus')
        PlayerService.playerStatusReceived(state)
        return state
    }

    static Stop = async() => {
        await PlayerService.makePlayerRequest(RequestType.Stop, 0)
    }

    static ToggleRepeat=async() => {
        await PlayerService.makePlayerControlRequest(ControlRequest.RepeatToggle, 0)
    }

    static SetVolume=async(volume: number)=> {
        await PlayerService.makePlayerControlRequest(ControlRequest.VolumeAbsolute,volume)
    }

    static Seek=async(position: number)=> {
        await PlayerService.makePlayerControlRequest(ControlRequest.Seek,position)
    }

    static EnqueueSong=async(id:number)=>{
        await PlayerService.makePlayerRequest(RequestType.Enqueue, id)
    }

    static PlayPause=async(id: number): Promise<IPlayerStatus> =>{
        const beforeState = await PlayerService.GetPlayerStatus();
        const shouldStop: boolean = (beforeState.playerState==PlaybackState.Playing||beforeState.playerState==PlaybackState.Paused)
                                    && (beforeState.currentLibraryItem && beforeState.currentLibraryItem.id!==id)
        if(shouldStop) {
            await PlayerService.Stop();
            var stopResult = await PlayerService.GetPlayerStatus()
            if(stopResult.playerState==PlaybackState.Stopped)
                await PlayerService.makePlayerRequest(RequestType.Play, id)
            else
                await PlayerService.GetPlayerStatus();
        } else {
            const operation: RequestType = (beforeState.playerState==PlaybackState.Stopped)
                ? RequestType.Play
                : (beforeState.playerState==PlaybackState.Playing?RequestType.Pause:RequestType.Resume)
            await PlayerService.makePlayerRequest(operation, id)
        }
        return await PlayerService.GetPlayerStatus();
    }
}
export default PlayerService