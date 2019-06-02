import * as React from 'react'
import { List, Spin, Button } from "antd"
import {ILibraryState} from '../Models/Library'
import { PlaybackState, IPlayerStatus } from '../Models/Player';
interface IMediaItemsGridProps {
    libraryStatus?: ILibraryState
    playerStatus?: IPlayerStatus
    playCommand: any
}
export class MediaItemsGrid extends React.Component<IMediaItemsGridProps> {
    constructor(props: IMediaItemsGridProps) {
        super(props)
    }
    render() {
        return (
            <List dataSource={this.props.libraryStatus.items}
                renderItem={item => (
                    <List.Item key={item.id}
                        actions={[
                            <Button
                                type={this.props.playerStatus.currentLibraryItem && this.props.playerStatus.currentLibraryItem.id===item.id && this.props.playerStatus.playerState===PlaybackState.Playing?'primary':'default' }
                                shape='circle' 
                                icon={this.props.playerStatus.currentLibraryItem && this.props.playerStatus.currentLibraryItem.id===item.id && this.props.playerStatus.playerState===PlaybackState.Playing?'pause-circle':'play-circle' }
                                onClick={()=>this.props.playCommand(item.id)}
                            />]
                        }>
                        <List.Item.Meta title={item.name} description={item.fullPath}/>
                    </List.Item>
                )}
            >
                {!this.props.libraryStatus.items && (
                    <div className="demo-loading-container">
                    <Spin />
                    </div>
                )}
            </List>
        )
    }
}