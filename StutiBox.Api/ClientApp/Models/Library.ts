interface IMediaItem {
    id: number
    name: string
    fullPath: string
    lengthBytes: number
    lengthSeconds: number
    lengthTimeString: string
    tags: string[]
}
interface ILibraryState {
    status?: boolean
    libraryRefreshedAt?: string
    items?: IMediaItem[]
}

export {IMediaItem, ILibraryState}