import * as React from 'react'
import {IHomeState} from '../Pages/Home'
import { Row, Col, Button, Slider, Switch, Icon, Statistic } from 'antd';
import { PlaybackState } from '../Models/Player';
const Countdown = Statistic.Countdown;

interface IPlayerHeaderProps {
    appState: IHomeState
    refreshCommand: any
    stopCommand: any
    playPauseCommand: any
    repeatToggleCommand: any
    volumeCommand: any
    seekCommand: any
}
export class PlayerHeader extends React.Component<IPlayerHeaderProps>{
    constructor(props) {
        super(props)
    }
    render() {
        return(
            <div>
                <Row>
                <Col xs={{span: 4}} sm={{span: 4}} md={{span: 4}} lg={{span: 4}} xl={{span: 2}} xxl={{span: 2}}>
                    <Button
                        type='primary'
                        size='large'
                        disabled={(this.props.appState.playerStatus&&this.props.appState.playerStatus.playerState!==PlaybackState.Paused)}
                        shape='circle'
                        icon='play-circle'
                        onClick={()=>this.props.playPauseCommand(this.props.appState.playerStatus.currentLibraryItem.id)}/>
                </Col>
                <Col xs={{span: 4}} sm={{span: 4}} md={{span: 4}} lg={{span: 4}} xl={{span: 2}} xxl={{span: 2}}>
                    <Button
                        type='primary'
                        size='large'
                        disabled={(this.props.appState.playerStatus&&this.props.appState.playerStatus.playerState!==PlaybackState.Playing)}
                        shape='circle'
                        icon='pause-circle'
                        onClick={()=>this.props.playPauseCommand(this.props.appState.playerStatus.currentLibraryItem.id)}/>
                </Col>
                <Col xs={{span: 4}} sm={{span: 4}} md={{span: 4}} lg={{span: 4}} xl={{span: 2}} xxl={{span: 2}}>
                    <Button
                        type='danger'
                        size='large'
                        disabled={!(this.props.appState.playerStatus&&(this.props.appState.playerStatus.playerState===PlaybackState.Playing||this.props.appState.playerStatus.playerState===PlaybackState.Paused))}
                        shape='circle'
                        icon='stop'
                        onClick={()=>this.props.stopCommand()}/>
                </Col>
                <Col xs={{span: 4}} sm={{span: 4}} md={{span: 4}} lg={{span: 4}} xl={{span: 2}} xxl={{span: 2}}>
                    <Switch style={{marginTop: "10px"}}
                        checked={(this.props.appState.playerStatus && this.props.appState.playerStatus.repeat)}
                        checkedChildren={<Icon type='retweet'/>}
                        unCheckedChildren={<Icon type='retweet'/>}
                        onClick={()=>this.props.repeatToggleCommand()}/>
                </Col>
                <Col xs={{span: 4}} sm={{span: 4}} md={{span: 4}} lg={{span: 4}} xl={{span: 6}} xxl={{span: 6}}>
                    <Slider
                        min={0} max={100}
                        style={{width: "100px"}} 
                        value={(this.props.appState.playerStatus)?this.props.appState.playerStatus.volume: 50}
                        onChange={(value)=>this.props.volumeCommand(value)}/>
                </Col>
                <Col xs={{span: 24}} sm={{span:24}} md={{span: 24}} lg={{span: 24}} xl={{span: 10}} xxl={{span: 10}}>
                    <Statistic
                        style={{marginLeft: "10px"}}
                        title="Volume"
                        value={(
                            this.props.appState.playerStatus)
                            ? this.props.appState.playerStatus.volume
                            : "0"
                        }/>
                </Col>
            </Row>
            <Row gutter={8}>
                <Col xs={{span: 12}} sm={{span: 12}} md={{span: 12}} lg={{span: 10}} xl={{span: 14}} xxl={{span: 14}}>
                    <Slider
                        disabled={!(this.props.appState.playerStatus && this.props.appState.playerStatus.playerState==PlaybackState.Playing)}
                        min={0}
                        max={
                                (this.props.appState.playerStatus&&this.props.appState.playerStatus.currentLibraryItem)
                                ? this.props.appState.playerStatus.currentLibraryItem.lengthSeconds
                                : 0
                            }
                        style={{width: "20rem"}} 
                        value={(this.props.appState.playerStatus)?this.props.appState.playerStatus.currentPositionSeconds: 50}
                        onChange={(value)=>this.props.seekCommand(value)}/>
                </Col>
                <Col xs={{span: 24}} sm={{span:24}} md={{span: 24}} lg={{span: 24}} xl={{span: 10}} xxl={{span: 10}}>
                    <Statistic
                        style={{marginLeft: "10px"}}
                        title={(
                            this.props.appState.playerStatus&&this.props.appState.playerStatus.currentLibraryItem)
                            ? this.props.appState.playerStatus.currentLibraryItem.name
                            : "No Song"
                        }
                        value={(
                            this.props.appState.playerStatus&&this.props.appState.playerStatus.currentLibraryItem)
                            ? this.props.appState.playerStatus.currentPositionString
                            : "--:--:--"
                        }/>
                </Col>
            </Row>
            </div>
        )
    }
}