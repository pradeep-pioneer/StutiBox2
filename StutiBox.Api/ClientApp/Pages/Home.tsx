import * as React from 'react'
import { Row, Col, Card, List, Spin, Icon, Button } from "antd"
import * as Axios from 'axios'
import "antd/dist/antd.css";
import "../Styles/common.less"
import "../Styles/custom.less"
import {PlaybackState, RequestType, IPlayerStatus} from '../Models/Player'
import {ILibraryState, IMediaItem} from '../Models/Library'
import LibraryService from '../Services/Library'
import PlayerService from '../Services/Player'
import {MediaItemsGrid} from '../Components/MediaItemsGrid'
import {MediaItemsHeader} from '../Components/MediaItemsHeader'
import {PlayerHeader} from '../Components/PlayerHeader'
export interface IHomeState {
    loaded: boolean
    libraryStatus?: ILibraryState
    playerStatus?: IPlayerStatus
}

class Home extends React.Component<{},IHomeState> {
    constructor(props) {
        super(props)
        this.state = {loaded: false, playerStatus:{playerState: PlaybackState.Stopped}, libraryStatus:{} }
    }

    async componentDidMount() {
        var libState = await LibraryService.getLibraryItems()
        var playerState = await PlayerService.GetPlayerStatus()
        this.setState({libraryStatus: libState, playerStatus: playerState, loaded: true})
        setInterval(()=>{
            if(this.state.playerStatus && this.state.playerStatus.playerState===PlaybackState.Playing){
                PlayerService.GetPlayerStatus()
                    .then(latestStatus=>this.setState({playerStatus:latestStatus}))
            }
        },1500)
    }
    playSong=(id: number)=>{
        PlayerService.PlayPause(id)
            .then(result=>{
                this.setState({playerStatus: result})
            })
    }

    stop=()=>{
        PlayerService.Stop()
            .then(result=>{
                this.setState({playerStatus:result})
            })
    }

    refreshLibrary=()=>{
        LibraryService.refreshLibrary()
            .then(result=>{
                this.setState({libraryStatus:result})
                console.log(result)
            })
    }

    toggleRepeat=()=>{
        PlayerService.ToggleRepeat()
            .then(result=>{this.setState({playerStatus: result})})
    }

    setVolume=(volume: number)=>{
        PlayerService.SetVolume(volume)
            .then(result=>{this.setState({playerStatus: result})})
    }

    seek=(position: number)=>{
        PlayerService.Seek(position)
            .then(result=>{this.setState({playerStatus: result})})
    }

    render() {
        return (
            <Row gutter={5} style={{margin: "10px"}}>
                <Col xs={{ span: 24 }} sm={{ span: 24 }} md={{ span: 12 }} lg={{ span: 12 }} xl={{ span: 12 }} xxl={{ span: 12 }}>
                    <Card
                        title={
                            <PlayerHeader 
                                appState={this.state} 
                                refreshCommand={this.refreshLibrary} 
                                stopCommand={this.stop}
                                playPauseCommand={this.playSong}
                                repeatToggleCommand={this.toggleRepeat}
                                volumeCommand={this.setVolume}
                                seekCommand={this.seek}/>
                            }
                        className="product-card">
                    </Card>
                </Col>
                <Col xs={{ span: 24 }} sm={{ span: 24 }} md={{ span: 12 }} lg={{ span: 12 }} xl={{ span: 12 }} xxl={{ span: 12 }}>
                    <Card
                        title={<MediaItemsHeader appState={this.state} refreshCommand={this.refreshLibrary} stopCommand={this.stop}/>}
                        className="product-card">
                            <MediaItemsGrid libraryStatus={this.state.libraryStatus} playerStatus={this.state.playerStatus} playCommand={this.playSong}/>
                    </Card>
                </Col>
            </Row>
        )
    }
}

export default Home