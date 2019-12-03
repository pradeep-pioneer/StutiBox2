import { IMediaItem } from "./Library";

enum PlaybackState {
    NotInitialized = -1,
    Faulted = 0,
    Stopped = 1,
    Playing = 2,
    Paused = 3
}

enum ControlRequest
{
    VolumeAbsolute = 1,
    RepeatToggle = 2,
    Random = 3,
    Seek = 4,
    VolumeRelative = 5,
    RepeatAbsolute = 6
}

enum RequestType
{
    Play = 0,
    Pause = 1,
    Stop = 2,
    Resume = 3,
    Enqueue = 4,
    DeQueue = 5
}
enum ChannelStates {
    NotInitialized=-0,
    Ready,
    Playing,
    Paused,
    Faulted
}
interface IPlayerStatus {
    status?: boolean
    totalLibraryItems?: number
    libraryRefreshedAt?: Date
    playerState: PlaybackState
    currentLibraryItem?: IMediaItem
    bassState?:ChannelStates
    volume?: number
    currentPositionBytes?: number
    currentPositionSeconds?: number
    currentPositionString?: string
    repeat?: boolean
    nowPlaying: IMediaItem[]
}
export {PlaybackState, RequestType, IPlayerStatus, ControlRequest}