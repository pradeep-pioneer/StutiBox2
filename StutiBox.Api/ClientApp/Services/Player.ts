import * as Axios from 'axios'
import {IPlayerStatus, PlaybackState, RequestType, ControlRequest} from '../Models/Player'
class PlayerService {
    private static async makePlayerRequest(requestType: RequestType, identifier: number) {
        return await Axios.default.post("/api/Player/Request", {requestType, identifier}, {"headers": {"accept": "*/*","Content-Type": "application/json-patch+json"}})
    }
    private static async makePlayerControlRequest(controlRequest: ControlRequest, requestData: any) {
        return await Axios.default.post("/api/Player/Control", {controlRequest, requestData}, {"headers": {"accept": "*/*","Content-Type": "application/json-patch+json"}})
    }
    static async GetPlayerStatus() {
        return Axios.default.get<{},Axios.AxiosResponse>(
            "/api/Player", 
            {
                "headers": {"accept": "*/*",
                "Content-Type": "application/json-patch+json"
            }}).then(response=>{
                if(response.status!==200 || !response.data.status)
                    throw new Error("Could not stop!")
                return response.data as IPlayerStatus
            })
    }

    static async Stop():Promise<IPlayerStatus> {
        const beforeState = await this.GetPlayerStatus();
        if(beforeState.playerState==PlaybackState.Paused||beforeState.playerState==PlaybackState.Playing){
            const response = await this.makePlayerRequest(RequestType.Stop, 0)
            if(response.status!==200 || !response.data.status)
                throw new Error("Could not stop!")
        }
        return await this.GetPlayerStatus()
    }

    static async ToggleRepeat() {
        const response = await this.makePlayerControlRequest(ControlRequest.RepeatToggle, null)
        if(response.status!==200 || !response.data.status)
            throw new Error("Could not stop!")
        return await this.GetPlayerStatus()
    }

    static async SetVolume(volume: number) {
        const response = await this.makePlayerControlRequest(ControlRequest.VolumeAbsolute,volume)
        if(response.status!==200 || !response.data.status)
            throw new Error("Could not stop!")
        return await this.GetPlayerStatus()
    }

    static async Seek(position: number) {
        const response = await this.makePlayerControlRequest(ControlRequest.Seek,position)
        if(response.status!==200 || !response.data.status)
            throw new Error("Could not stop!")
        return await this.GetPlayerStatus()
    }

    static async PlayPause(id: number): Promise<IPlayerStatus> {
        const beforeState = await this.GetPlayerStatus();
        const shouldStop: boolean = (beforeState.playerState==PlaybackState.Playing||beforeState.playerState==PlaybackState.Paused)
                                    && (beforeState.currentLibraryItem && beforeState.currentLibraryItem.id!==id)
        if(shouldStop) {
            var stopResult = await this.Stop();
            if(stopResult.playerState==PlaybackState.Stopped){
                const response = await this.makePlayerRequest(RequestType.Play, id)
                if(response.status==200){
                    if(!response.data.status)
                        throw new Error(response.data.messages)
                }else 
                    throw new Error(response.statusText)
            }
        } else {
            const operation: RequestType = (beforeState.playerState==PlaybackState.Stopped)
                ? RequestType.Play
                : (beforeState.playerState==PlaybackState.Playing?RequestType.Pause:RequestType.Resume)
            var response = await this.makePlayerRequest(operation, id)
            if(response.status==200){
                if(!response.data.status)
                    throw new Error(response.data.messages)
            } else 
                throw new Error(response.statusText)
        }
        return await this.GetPlayerStatus();
    }
}
export default PlayerService