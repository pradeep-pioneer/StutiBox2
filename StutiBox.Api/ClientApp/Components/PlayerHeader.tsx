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
                <Col xs={{span: 4}} sm={{span: 2}} md={{span: 2}} lg={{span: 2}} xl={{span: 1}} xxl={{span: 1}}>
                    <Button
                        type='primary'
                        size='large'
                        disabled={(this.props.appState.playerStatus&&this.props.appState.playerStatus.playerState!==PlaybackState.Paused)}
                        shape='circle'
                        icon='play-circle'
                        onClick={async()=>await this.props.playPauseCommand(this.props.appState.playerStatus.currentLibraryItem.id)}/>
                </Col>
                <Col xs={{span: 4}} sm={{span: 2}} md={{span: 2}} lg={{span: 2}} xl={{span: 1}} xxl={{span: 1}}>
                    <Button
                        type='primary'
                        size='large'
                        disabled={(this.props.appState.playerStatus&&this.props.appState.playerStatus.playerState!==PlaybackState.Playing)}
                        shape='circle'
                        icon='pause-circle'
                        onClick={async()=>await this.props.playPauseCommand(this.props.appState.playerStatus.currentLibraryItem.id)}/>
                </Col>
                <Col xs={{span: 4}} sm={{span: 2}} md={{span: 2}} lg={{span: 2}} xl={{span: 1}} xxl={{span: 1}}>
                    <Button
                        type='danger'
                        size='large'
                        disabled={!(this.props.appState.playerStatus&&(this.props.appState.playerStatus.playerState===PlaybackState.Playing||this.props.appState.playerStatus.playerState===PlaybackState.Paused))}
                        shape='circle'
                        icon='stop'
                        onClick={async()=>await this.props.stopCommand()}/>
                </Col>
                <Col xs={{span: 4}} sm={{span: 2}} md={{span: 2}} lg={{span: 2}} xl={{span: 1}} xxl={{span: 1}}>
                    <Switch style={{marginTop: "10px"}}
                        checked={(this.props.appState.playerStatus && this.props.appState.playerStatus.repeat)}
                        checkedChildren={<Icon type='retweet'/>}
                        unCheckedChildren={<Icon type='retweet'/>}
                        onClick={async()=>await this.props.repeatToggleCommand()}/>
                </Col>
                <Col xs={{span: 6}} sm={{span: 16}} md={{span: 4}} lg={{span: 2}} xl={{span: 2}} xxl={{span: 2}}>
                    <Slider
                        min={0} max={100}
                        value={(this.props.appState.playerStatus)?this.props.appState.playerStatus.volume: 50}
                        onChange={async(value)=>await this.props.volumeCommand(value)}/>
                </Col>
                <Col xs={{span: 2}} sm={{span:4}} md={{span: 2}} lg={{span: 1}} xl={{span: 1}} xxl={{span: 1}}>
                    <div style={{padding: "0.5rem"}}>{(this.props.appState.playerStatus)? this.props.appState.playerStatus.volume:"0"}</div>
                </Col>
                <Col xs={{span: 24}} sm={{span: 24}} md={{span: 10}} lg={{span: 12}} xl={{span: 6}} xxl={{span: 6}}>
                    <Slider
                        disabled={!(this.props.appState.playerStatus && this.props.appState.playerStatus.playerState==PlaybackState.Playing)}
                        min={0}
                        max={
                                (this.props.appState.playerStatus&&this.props.appState.playerStatus.currentLibraryItem)
                                ? this.props.appState.playerStatus.currentLibraryItem.lengthSeconds
                                : 0
                            }
                        value={(this.props.appState.playerStatus)?this.props.appState.playerStatus.currentPositionSeconds: 50}
                        onChange={async(value)=>await this.props.seekCommand(value)}/>
                </Col>
                <Col xs={{span: 18}} sm={{span:18}} md={{span: 18}} lg={{span: 18}} xl={{span: 8}} xxl={{span: 6}}>
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
                <Col xs={{span: 6}} sm={{span:6}} md={{span: 6}} lg={{span: 6}} xl={{span: 2}} xxl={{span: 4}}>
                    <Statistic
                        style={{marginLeft: "10px"}}
                        title={(
                            this.props.appState.playerStatus&&this.props.appState.playerStatus.currentLibraryItem)
                            ? "Duration"
                            : "No Song"
                        }
                        value={(
                            this.props.appState.playerStatus&&this.props.appState.playerStatus.currentLibraryItem)
                            ? this.props.appState.playerStatus.currentLibraryItem.lengthTimeString
                            : "--:--:--"
                        }/>
                </Col>
            </Row>
            </div>
        )
    }
}