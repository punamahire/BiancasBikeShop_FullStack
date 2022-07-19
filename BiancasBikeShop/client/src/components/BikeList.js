import { useState, useEffect } from "react"
import { Button } from "reactstrap"
import BikeCard from "./BikeCard"
import { getBikes, getBikesInShopCount } from "../bikeManager"

export default function BikeList({ setDetailsBikeId }) {
    const [bikes, setBikes] = useState([]);
    const [bikesInShop, setBikesInShop] = useState();

    const getAllBikes = () => {
        getBikes().then(bikes => setBikes(bikes));
    }

    const getNumberOfBikes = () => {
        getBikesInShopCount().then(numOfBikes => setBikesInShop(numOfBikes));
    }

    useEffect(() => {
        getAllBikes()
    }, [])
    return (
        <>
            <div>
                <Button className="mt-2 mb-2" color="dark" onClick={() => {
                    getNumberOfBikes();
                }}>
                    Bikes In Shop
                </Button>
                {bikesInShop &&
                    <p><strong>No. of bikes in shop: {bikesInShop}</strong></p>
                }
            </div>
            <h2>Bikes</h2>
            <div className="instrument-list">
                {bikes.map(bike =>
                    <BikeCard
                        key={bike.id}
                        bike={bike}
                        setDetailsBikeId={setDetailsBikeId}
                        allowEdit={true} />
                )}
            </div>
        </>
    )
}