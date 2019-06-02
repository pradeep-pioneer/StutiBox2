import * as React from 'react'
import {IHomeState} from '../Pages/Home'
import { Row, Col, Badge, Button, Spin } from 'antd';
import { PlaybackState } from '../Models/Player';
interface IMediaItemsHeaderProps {
    appState: IHomeState
    refreshCommand: any
    stopCommand: any
}
export class MediaItemsHeader extends React.Component<IMediaItemsHeaderProps> {
    constructor(props: IMediaItemsHeaderProps){
        super(props)
    }
    render(){
        return (
            <Row gutter={2} style={{margin:"10px 10px"}}>
                <Col xs={{span: 12}} sm={{span:8}} md={{span: 8}} lg={{span: 6}} xl={{span: 6}} xxl={{span: 6}}>
                    <Badge
                        count={(this.props.appState.libraryStatus && this.props.appState.libraryStatus.items)?this.props.appState.libraryStatus.items.length:0}
                        style={{ backgroundColor: '#52c41a' }}>
                            <h3 style={{marginRight:"10px", marginTop: "10px"}}>Songs</h3>
                        </Badge>
                </Col>
                <Col xs={{span: 4}} sm={{span:2}} md={{span: 3}} lg={{span: 2}} xl={{span: 2}} xxl={{span: 2}}>
                    <Spin
                        spinning={this.props.appState.playerStatus && this.props.appState.playerStatus.playerState===PlaybackState.Playing}
                        size='large'/>
                </Col>
                <Col xs={{span: 4}} sm={{span:2}} md={{span: 3}} lg={{span: 2}} xl={{span: 2}} xxl={{span: 2}}>
                    <Button
                        type='danger' 
                        shape='circle'
                        icon='stop'
                        style={{margin: "0 auto"}}
                        onClick={()=>this.props.stopCommand()}/>
                </Col>
                <Col xs={{span: 4}} sm={{span:2}} md={{span: 3}} lg={{span: 2}} xl={{span: 2}} xxl={{span: 2}}>
                    <Button
                        type='primary' 
                        shape='circle'
                        icon='reload'
                        style={{margin: "0 auto"}}
                        onClick={()=>this.props.refreshCommand()}/>
                </Col>
            </Row>
        )
    }
}