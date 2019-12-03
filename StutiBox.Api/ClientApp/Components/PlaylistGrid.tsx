import * as React from 'react'
import { List, Spin, Button } from "antd"
import { IMediaItem } from '../Models/Library';
interface IPlaylistGridProps {
    playlist: IMediaItem[]
}
export class PlaylistGrid extends React.Component<IPlaylistGridProps> {
    constructor(props: IPlaylistGridProps) {
        super(props)
    }
    render() {
        return (
            this.props.playlist &&
            <List dataSource={this.props.playlist}
                renderItem={item => (
                    <List.Item key={item.id}>
                        <List.Item.Meta title={item.name} description={item.fullPath}/>
                        <div><List.Item.Meta title={item.lengthTimeString}/></div>
                    </List.Item>
                )}
            >
                {!this.props.playlist && (
                    <div className="demo-loading-container">
                    <Spin />
                    </div>
                )}
            </List>
        )
    }
}