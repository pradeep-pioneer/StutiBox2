import * as React from "react";
import { Row, Col, Card, Button, Modal, Spin } from "antd";
import "antd/dist/antd.css";
import "../Styles/common.less";
import "../Styles/custom.less";
import { PlaybackState, IPlayerStatus } from "../Models/Player";
import { ILibraryState } from "../Models/Library";
import LibraryService from "../Services/Library";
import PlayerService from "../Services/Player";
import { MediaItemsGrid } from "../Components/MediaItemsGrid";
import { PlayerHeader } from "../Components/PlayerHeader";
import { PhotoSlider } from "../Components/PhotoSlider";
import {PlaylistGrid} from "../Components/PlaylistGrid";

export interface IHomeState {
  libraryStatus?: ILibraryState;
  showPlayer: boolean;
  showPlaylist: boolean;
  message?: string;
  playerStatus?: IPlayerStatus;
}

class Home extends React.Component<{}, IHomeState> {
  constructor(props) {
    super(props);
    this.state = { showPlayer: false, showPlaylist: false };
  }

  async componentDidMount() {
    await LibraryService.initialize(this.updateLibraryState);
    await PlayerService.initialize(this.updatePlayerState);
  }
  updateLibraryState = (status: ILibraryState) => {
    this.setState({ libraryStatus: status });
  };
  updatePlayerState = (status: IPlayerStatus) => {
    this.setState({ playerStatus: status });
  };
  playSong = async (id: number) => {
    await PlayerService.PlayPause(id);
  };

  enqueueSong=async(id: number) => {
    await PlayerService.EnqueueSong(id);
  }

  stop = async () => {
    await PlayerService.Stop();
  };

  refreshLibrary = () => {
    LibraryService.refreshLibrary();
  };

  toggleRepeat = async () => {
    await PlayerService.ToggleRepeat();
  };

  setVolume = async (volume: number) => {
    await PlayerService.SetVolume(volume);
  };

  seek = async (position: number) => {
    await PlayerService.Seek(position);
  };

  playerModal = (show: boolean) => {
    this.setState({ showPlayer: show });
  };

  nowPlayingModal = (show: boolean) => {
    this.setState({showPlaylist: show});
  }

  isLoaded = () => {
    return this.state.libraryStatus && this.state.playerStatus ? true : false;
  };

  render() {
    if (this.isLoaded()) {
      return (
        <Row>
          <Row>
            <Col>
              <div style={{ position: "absolute", zIndex: 2, top: "2rem", left: "2rem" }}>
                <Button
                  size="large"
                  type="default"
                  shape="circle-outline"
                  icon={
                    this.state.playerStatus.playerState
                      ? this.state.playerStatus.playerState !=
                        PlaybackState.Playing
                        ? "play-circle"
                        : ""
                      : "play-circle"
                  }
                  onClick={() => this.playerModal(true)}
                >
                  <Spin
                    size="small"
                    spinning={
                      this.state.playerStatus.playerState
                        ? this.state.playerStatus.playerState ==
                          PlaybackState.Playing
                        : false
                    }
                  />
                </Button>
                <Button
                  size="large"
                  type="primary"
                  shape="circle-outline"
                  icon="play-circle" onClick={() => this.nowPlayingModal(true)}>

                  </Button>
              </div>
              <Card className="product-card">
                <PhotoSlider />
              </Card>
            </Col>
          </Row>
          <Modal
            width="95%"
            style={{ top: "1rem"}}
            visible={this.state.showPlaylist}
            onCancel={() => this.nowPlayingModal(false)}
            onOk={() => this.nowPlayingModal(false)}>
              <PlaylistGrid playlist={this.state.playerStatus.nowPlaying}/>
            </Modal>
          <Modal
            width="95%"
            style={{ top: "1rem"}}
            visible={this.state.showPlayer}
            onCancel={() => this.playerModal(false)}
            onOk={() => this.playerModal(false)}
          >
            <Card
              title={
                <PlayerHeader
                  appState={this.state}
                  refreshCommand={this.refreshLibrary}
                  stopCommand={this.stop}
                  playPauseCommand={this.playSong}
                  repeatToggleCommand={this.toggleRepeat}
                  volumeCommand={this.setVolume}
                  seekCommand={this.seek}
                />
              }
              className="product-card"
            >
              <MediaItemsGrid enqueueCommand={this.enqueueSong}
                libraryStatus={this.state.libraryStatus}
                playerStatus={this.state.playerStatus}
                playCommand={this.playSong}
              />
            </Card>
          </Modal>
        </Row>
      );
    } else {
      return <Spin size="large" spinning={this.isLoaded()} />;
    }
  }
}

export default Home;
